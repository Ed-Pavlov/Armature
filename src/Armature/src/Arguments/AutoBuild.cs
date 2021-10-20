using Armature.Core;

namespace Armature
{
  public static class AutoBuild
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
      private const short ByNameWeight = 10;
      private const short ByTypeWeight = 5;
      
      public IArgumentTuner Type { get; } = new ArgumentTuner(
        node => node
               .GetOrAddNode(new IfFirstUnit(Static<IsParameterInfo>.Instance, ByTypeWeight))
               .UseBuildAction(Static<BuildArgumentByParameterType>.Instance, BuildStage.Create));

      public IArgumentTuner Name { get; } = new ArgumentTuner(
        node => node
               .GetOrAddNode(new IfFirstUnit(Static<IsParameterInfo>.Instance, ByNameWeight))
               .UseBuildAction(Static<BuildArgumentByParameterName>.Instance, BuildStage.Create));
    }
  }
}
