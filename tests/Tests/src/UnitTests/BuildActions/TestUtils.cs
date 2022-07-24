using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.UnitTests.BuildActions;

public static class TestUtils
{
  public static BuildResult       ToArguments<T>(this T?      value)  => new BuildResult(value?.GetType().IsArray == true ? value : new object?[] {value});
  public static BuildResult       ToBuildResult(this  object? value)  => new BuildResult(value);
  public static BuildChain ToBuildChain(this   UnitId  unitId) => new BuildChain(new[] {unitId}, 0);
}

public static class Kind
{
  public static UnitId Is<T>()               => new UnitId(typeof(T), null);
  public static UnitId Is(object?      kind) => new UnitId(kind, null);
  public static UnitId Tag(this UnitId unitId, object? tag) => new UnitId(unitId.Kind, tag);
}