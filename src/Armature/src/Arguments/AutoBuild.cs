using Armature.Core;

namespace Armature
{
  public class AutoBuild
  {
    public static ByParam   ByParameter      => Static<ByParam>.Instance;
    public static ParamList MethodParameters => Static<ParamList>.Instance;

    public class ParamList
    {
      public IArgumentTuner InDirectOrder { get; } = new ArgumentTuner(
        node => node
               .GetOrAddNode(new IfFirstUnit(Static<IsParameterInfoList>.Instance))
               .UseBuildAction(Static<BuildMethodArgumentsInDirectOrder>.Instance, BuildStage.Create));
    }

    public class ByParam
    {
      public IArgumentTuner Type { get; } = new ArgumentTuner(
        node => node
               .GetOrAddNode(new IfFirstUnit(Static<IsParameterInfo>.Instance))
               .UseBuildAction(Static<BuildArgumentByParameterType>.Instance, BuildStage.Create));

      public IArgumentTuner Name { get; } = new ArgumentTuner(
        node => node
               .GetOrAddNode(new IfFirstUnit(Static<IsParameterInfo>.Instance))
               .UseBuildAction(Static<BuildArgumentByParameterName>.Instance, BuildStage.Create));
    }
  }
}
