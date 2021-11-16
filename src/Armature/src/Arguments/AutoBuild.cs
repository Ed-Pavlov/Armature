using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public static class AutoBuild
{
  public static ByParam   ByParameter      => Static.Of<ByParam>();
  public static ParamList MethodParameters => Static.Of<ParamList>();

  public class ParamList
  {
    public IArgumentTuner InDirectOrder { get; } = new ArgumentTuner(
      node => node
             .GetOrAddNode(new IfFirstUnitBuildChain(Static.Of<IsParameterInfoList>()))
             .UseBuildAction(Static.Of<BuildMethodArgumentsInDirectOrder>(), BuildStage.Create));
  }

  public class ByParam
  {
    private const short ByNameWeight = 10;
    private const short ByTypeWeight = 5;

    public IArgumentTuner Type { get; } = new ArgumentTuner(
      node => node
             .GetOrAddNode(new IfFirstUnitBuildChain(Static.Of<IsParameterInfo>(), WeightOf.BuildContextPattern.IfFirstUnit + ByTypeWeight))
             .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create));

    public IArgumentTuner Name { get; } = new ArgumentTuner(
      node => node
             .GetOrAddNode(new IfFirstUnitBuildChain(Static.Of<IsParameterInfo>(), WeightOf.BuildContextPattern.IfFirstUnit + ByNameWeight))
             .UseBuildAction(Static.Of<BuildArgumentByParameterName>(), BuildStage.Create));
  }
}