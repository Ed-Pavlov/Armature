using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Gets a constructor of type which matches specified parameter types list.
/// </summary>
public record GetConstructorByParameterTypes : IBuildAction, ILogString
{
  private readonly Type[] _parameterTypes;

  public GetConstructorByParameterTypes(params Type[] parameterTypes)
  {
    _parameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));
    if(parameterTypes.Any(_ => _ is null)) throw new ArgumentNullException(nameof(parameterTypes), "One or more items is null");
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.BuildChain.TargetUnit.GetUnitType();
    var ctor     = GetConstructor(unitType);

    if(ctor is not null)
      buildSession.BuildResult = new BuildResult(ctor);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  private ConstructorInfo? GetConstructor(Type unitType)
    => unitType.GetConstructors().FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameterTypes));

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
