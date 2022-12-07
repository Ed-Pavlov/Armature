#nullable enable
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.Extensibility.MaybePropagation.Implementation;

public class BuildMaybe : IBuildAction
{
  private object? _tag;

  public BuildMaybe(object? tag) => _tag = tag;

  public void Process(IBuildSession buildSession)
  {
    var maybeType = buildSession.Stack.TargetUnit.GetUnitType();
    var valueType = maybeType.GenericTypeArguments[0];

    var buildResult = buildSession.BuildUnit(new UnitId(valueType, _tag));

    if(buildResult.HasValue)
    {
      var constructor = maybeType.GetConstructor(new[]{valueType})!;
      var maybe       = constructor.Invoke(new[] {buildResult.Value});
      buildSession.BuildResult = new BuildResult(maybe);
    }
  }

  public void PostProcess(IBuildSession buildSession){}
}