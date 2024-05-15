using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace Tests.Util;

public static class TestUtil
{
  [DebuggerStepThrough]
  public static BuildSession.Stack CreateBuildStack(params UnitId[] array) => new BuildSession.Stack(array);

  [DebuggerStepThrough]
  public static BuildSession.Stack ToBuildStack(this UnitId item) => new(new []{item});

  [DebuggerStepThrough]
  public static IEnumerable<UnitId> AsEnumerable(this BuildSession.Stack stack) => new BuildStackEnumerable(stack);

  [DebuggerStepThrough]
  public static BuildResult ToArguments<T>(this T? value) => new BuildResult(value?.GetType().IsArray == true ? value : new object?[] {value});

  [DebuggerStepThrough]
  public static BuildResult ToBuildResult(this object? value) => new BuildResult(value);

  public record OtherUnitPattern : IUnitPattern
  {
    public bool Matches(UnitId unitId) => throw new NotSupportedException();
  }

  private class BuildStackEnumerable : IEnumerable<UnitId>
  {
    private readonly BuildSession.Stack _stack;

    public BuildStackEnumerable(BuildSession.Stack stack) => _stack = stack;
    public IEnumerator<UnitId> GetEnumerator() => new Enumerator(_stack);

    private struct Enumerator : IEnumerator<UnitId>
    {
      private readonly BuildSession.Stack _buildStack;

      private bool   _disposed;
      private int    _iterator;
      private UnitId _current;

      public Enumerator(BuildSession.Stack stack)
      {
        _buildStack = stack;
        Reset();
      }

      public bool MoveNext()
      {
        if(_disposed) throw new ObjectDisposedException(nameof(Enumerator));

        if(_iterator >= _buildStack.Count)
        {
          _current = default;
          return false;
        }

        _current = _buildStack[_iterator++];
        return true;
      }

      public UnitId Current
      {
        get
        {
          if(_disposed) throw new ObjectDisposedException(nameof(Enumerator));
          return _current;
        }
      }

      public void Reset()
      {
        _current  = default;
        _iterator = 0;
      }

      public void Dispose() => _disposed = true;

      object IEnumerator.Current => Current;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }

  public static IEnumerable<Tag> all_special_tags()
  {
    yield return ServiceTag.Argument;
    yield return ServiceTag.Constructor;
    yield return ServiceTag.PropertyCollection;
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
