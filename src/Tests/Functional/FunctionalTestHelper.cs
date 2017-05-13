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
      treatAll.AddChildBuildStep(new FindLongestConstructorBuildStep());
      treatAll.AddChildBuildStep(new FindAttributedConstructorBuildStep(10));
      treatAll.AddChildBuildStep(BuildValueForParameterStep.Instance);

      container.AddBuildStep(treatAll);
      return container;
    }
  }
}