using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.Parameters;
using Armature.Framework.Properties;

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

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance, ParameterMatchingWeight.Lowest)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              RedirectParameterInjectPointToTypeAndTokenBuildAction.Instance,
              new RedirectParameterToTypeAndTokenBuildAction()
            }),
        
        new LastUnitSequenceMatcher(PropetyValueMatcher.Instance, ParameterMatchingWeight.Lowest)
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