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
      BuildStage.Intercept,
      BuildStage.Redirect,
      BuildStage.Initialize,
      BuildStage.Create);

    public static Builder CreateBuilder(Builder[] parentBuilders, params object[] stages)
    {
      var container = new Builder(stages, parentBuilders);

      var treatAll = new AnyUnitSequenceMatcher();

      var constructorMathcer =
        new LeafUnitSequenceMatcher(AnyConstructorMatcher.Instance, FindConstructorBuildStepWeight.Lowest)
        .AddBuildAction(BuildStage.Create, new GetLongesConstructorBuildAction(), FindConstructorBuildStepWeight.Lowest);
      treatAll.Children.Add(constructorMathcer);

      var constructorByInjectPointIdMatcher = new ConstructorByInjectPointIdMatcher();
      var ctorByInjectPointIdMatcher = 
        new LeafUnitSequenceMatcher(constructorByInjectPointIdMatcher, FindConstructorBuildStepWeight.Attributed)
        .AddBuildAction(BuildStage.Create, constructorByInjectPointIdMatcher.BuildAction, FindConstructorBuildStepWeight.Attributed);
      treatAll.Children.Add(ctorByInjectPointIdMatcher);

      var anyParameterMatcher =
        new LeafUnitSequenceMatcher(AnyParameterMatcher.Instance, ParameterMatcherWeight.Lowest)
        .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction(), ParameterMatcherWeight.Lowest);
      treatAll.Children.Add(anyParameterMatcher);
      
      var injectPointParameterMatcher =
        new LeafUnitSequenceMatcher(InjectPointParameterMatcher.Instance, ParameterMatcherWeight.Lowest + 1)
        .AddBuildAction(BuildStage.Create, InjectPointParameterMatcher.BuildAction, 0);
      treatAll.Children.Add(injectPointParameterMatcher);
      
      container.AddUnitMatcher(treatAll);
      return container;
    }
  }
}