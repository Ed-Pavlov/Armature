using Armature.Core;
using Armature.Framework;

namespace Tests.Functional
{
  public static class FunctionalTestHelper
  {
    public static Builder CreateContainer(Builder parentBuilder = null)
    {
      return CreateContainer(parentBuilder, BuildStage.Cache, BuildStage.Redirect, BuildStage.Initialize, BuildStage.Create);
    }

    public static Builder CreateContainer(Builder parentBuilder = null, params object[] stages)
    {
      var container = new Builder(stages, parentBuilder);

      var treatAll = new AnyUnitBuildStep();
      treatAll.AddBuildStep(new FindLongestConstructorBuildStep(FindConstructorBuildStepWeight.Lowest));
      treatAll.AddBuildStep(new FindAttributedConstructorBuildStep(FindConstructorBuildStepWeight.Attributed));
      treatAll.AddBuildStep(new BuildValueForParameterStep(ParameterValueBuildActionWeight.Lowest));

      container.AddBuildStep(treatAll);
      return container;
    }
  }
}