using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.Util;

public static class TestUtil
{
  [DebuggerStepThrough]
  public static BuildSession.Stack CreateBuildStack(params UnitId[] array) => new BuildSession.Stack(array);

  [DebuggerStepThrough]
  public static BuildSession.Stack ToBuildStack(this UnitId item) => new(new []{item});

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
/// Increase readability of tests
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class TUnit
{
  public static UnitId OfType<T>()            => OfType<T>(null);
  public static UnitId OfType<T>(object? tag) => Unit.Of(typeof(T), tag);
}

/// <summary>
/// "No Type" type
/// </summary>
internal class Void
{
}
