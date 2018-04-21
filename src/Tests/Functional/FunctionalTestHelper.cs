using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;

namespace Tests.Functional
{
  public static class FunctionalTestHelper
  {
    public static Builder CreateEmptyBuilder(params object[] stages)
    {
      if(stages.Length == 0)
        stages = new object[]{BuildStage.Cache,
          BuildStage.Initialize,
          BuildStage.Create};
      
      return new Builder(stages);
    }

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
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            }),

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              CreateParameterValueForInjectPointBuildAction.Instance,
              new CreateParameterValueBuildAction()
            }),
        
        new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new CreatePropertyValueBuildAction()
            }),
      };

      var container = new Builder(stages, parentBuilders);
      container.Children.Add(treatAll);
      return container;
    }
    
    public static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            }),

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              CreateParameterValueForInjectPointBuildAction.Instance,
              new CreateParameterValueBuildAction()
            }),
        
        new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new CreatePropertyValueBuildAction()
            }),
      };

      var container = new Builder(new[]{ BuildStage.Cache, BuildStage.Initialize, BuildStage.Create});
      container.Children.Add(treatAll);
      return container;
    }
  }
}