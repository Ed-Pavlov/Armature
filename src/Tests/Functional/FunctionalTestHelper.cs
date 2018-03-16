using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;

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
        new LeafUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(),
              new GetLongesConstructorBuildAction()
            }),

        new LeafUnitSequenceMatcher(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new RedirectParameterInfoToTypeAndTokenBuildAction(),
              new RedirectParameterInfoBuildAction()
            })
      };

      var container = new Builder(stages, parentBuilders);
      container.AddUnitMatcher(treatAll);
      return container;
    }
  }
}