using System;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.Util;

public static class TestUtil
{
  [DebuggerStepThrough]
  public static BuildChain CreateBuildChain(params UnitId[] array) => new BuildChain(array);

  [DebuggerStepThrough]
  public static BuildChain ToBuildChain(this UnitId item) => new(new []{item});

  [DebuggerStepThrough]
  public static BuildResult ToArguments<T>(this T? value) => new BuildResult(value?.GetType().IsArray == true ? value : new object?[] {value});

  [DebuggerStepThrough]
  public static BuildResult ToBuildResult(this object? value) => new BuildResult(value);

  public record OtherUnitPattern : IUnitPattern
  {
    public bool Matches(UnitId unitId) => throw new NotSupportedException();
  }

  public static IEnumerable<SpecialTag> all_special_tags()
  {
    yield return Tag.Any;
    yield return SpecialTag.Argument;
    yield return SpecialTag.Constructor;
    yield return SpecialTag.Propagate;
    yield return SpecialTag.PropertyCollection;
  }
}

/// <summary>
/// An attempt to increase readability of tests
/// </summary>
public static class Kind
{
  public static UnitId Is<T>()                              => new UnitId(typeof(T), null);
  public static UnitId Is(Type         type)                => new UnitId(type, null);
  public static UnitId Is(object?      kind)                => new UnitId(kind, null);
  public static UnitId Tag(this UnitId unitId, object? tag) => new UnitId(unitId.Kind, tag);
}

/// <summary>
/// "No Type" type
/// </summary>
internal class Unit
{
}
