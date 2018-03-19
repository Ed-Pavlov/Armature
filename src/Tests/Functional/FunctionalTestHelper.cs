using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Constructor;
using Armature.Framework.BuildActions.Parameter;
using Armature.Framework.BuildActions.Property;
using Armature.Framework.UnitMatchers;
using Armature.Framework.UnitMatchers.Parameters;
using Armature.Framework.UnitMatchers.Properties;
using Armature.Framework.UnitSequenceMatcher;

namespace Tests.Functional
{
  public static class FunctionalTestHelper
  {
    public static Builder CreateBuilder(params Builder[] parentBuilders) => CreateBuilder(
      parentBuilders,
      BuildStage.Cache,
      BuildStage.Initialize,
      BuildStage.Create);

    public static Builder CreateBuilder(Builder[] parentBuilders, params object[] stages)
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(),
              GetLongesConstructorBuildAction.Instance
            }),

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance, InjectPointMatchingWeight.Lowest)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              RedirectParameterInjectPointToTypeAndTokenBuildAction.Instance,
              new RedirectParameterToTypeAndTokenBuildAction()
            }),
        
        new LastUnitSequenceMatcher(PropetyValueMatcher.Instance, InjectPointMatchingWeight.Lowest)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new RedirectPropertyInfoBuildAction()
            }),
      };

      var container = new Builder(stages, parentBuilders);
      container.AddUnitMatcher(treatAll);
      return container;
    }
  }
}