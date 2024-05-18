using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Gets the constructor of the type matches specified parameter types list.
/// </summary>
public record GetConstructorByParameterTypes : IBuildAction, ILogString
{
  private readonly BindingFlags _bindingFlags;
  private readonly Type[] _parameterTypes;

  public GetConstructorByParameterTypes(params Type[] parameterTypes) : this(BindingFlags.Instance | BindingFlags.Public, parameterTypes) { }
  public GetConstructorByParameterTypes(BindingFlags bindingFlags, params Type[] parameterTypes)
  {
    _parameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));
    if(parameterTypes.Any(_ => _ is null)) throw new ArgumentNullException(nameof(parameterTypes), "One or more items is null");

    _bindingFlags = bindingFlags;
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();
    var ctor     = GetConstructor(unitType, _bindingFlags);

    ctor.WriteToLog(LogLevel.Trace);
    if(ctor is not null)
      buildSession.BuildResult = new BuildResult(ctor);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  private ConstructorInfo? GetConstructor(Type unitType, BindingFlags bindingFlags)
    => unitType.GetConstructors(bindingFlags).FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameterTypes));

  private static bool IsParametersListMatch(ParameterInfo[] parameterInfos, Type[] parameterTypes)
  {
    if(parameterInfos.Length != parameterTypes.Length)
      return false;

    for(var i = 0; i < parameterInfos.Length; i++)
      if(parameterInfos[i].ParameterType != parameterTypes[i])
        return false;

    return true;
  }

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetConstructorByParameterTypes)} {{ Types: {_parameterTypes.ToHoconArray()} }} }}";

  public virtual bool Equals(GetConstructorByParameterTypes? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;
    if(_parameterTypes.Length != other._parameterTypes.Length) return false;

    for(var i = 0; i < _parameterTypes.Length; i++)
      if(_parameterTypes[i] != other._parameterTypes[i])
        return false;

    return true;
  }


  public override int GetHashCode()
  {
    var hash = _parameterTypes.Length.GetHashCode();

    foreach(var type in _parameterTypes)
      hash ^= type.GetHashCode();

    return hash;
  }
}