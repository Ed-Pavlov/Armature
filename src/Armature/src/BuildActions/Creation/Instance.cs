﻿using System.Diagnostics;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Caches passed object and set it as <see cref="BuildResult" /> in <see cref="Process" />.
/// </summary>
public record Instance<T> : IBuildAction, ILogString
{
  private readonly T _value;

  [DebuggerStepThrough]
  public Instance(T value) => _value = value;

  public void Process(IBuildSession buildSession) => buildSession.BuildResult = new BuildResult(_value);

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().Quote()} {{ Instance: {_value.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
