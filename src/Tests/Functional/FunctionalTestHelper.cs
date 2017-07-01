using Armature.Core;
using Armature.Framework;

namespace Tests.Functional
{
  public static class FunctionalTestHelper
  {
    public static Builder CreateBuilder(params Builder[] parentBuilders)
    {
      return CreateBuilder(parentBuilders, BuildStage.Cache, BuildStage.Redirect, BuildStage.Initialize, BuildStage.Create);
    }

    public static Builder CreateBuilder(Builder[] parentBuilders, params object[] stages)
    {
      var container = new Builder(stages, parentBuilders);

      var treatAll = new AnyUnitBuildStep();
      treatAll.AddBuildStep(new FindLongestConstructorBuildStep(FindConstructorBuildStepWeight.Lowest));
      treatAll.AddBuildStep(new FindAttributedConstructorBuildStep(FindConstructorBuildStepWeight.Attributed));
      treatAll.AddBuildStep(new BuildValueForParameterStep(ParameterValueBuildStepWeight.Lowest));

      container.AddBuildStep(treatAll);
      return container;
    }
  }
}