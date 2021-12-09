using System;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// "Builds" a property Unit of the currently building Unit of specified type
/// specified <see cref="InjectAttribute.InjectionPointId" />
/// </summary>
public record GetPropertyByType : IBuildAction, ILogString
{
  private readonly Type _type;

  public GetPropertyByType(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

  public void Process(IBuildSession buildSession)
  {
    var unitType   = buildSession.BuildChain.TargetUnit.GetUnitType();
    var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();

    buildSession.BuildResult =
      properties.Length switch
      {
        0 => throw new ArmatureException($"No property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}'"),
        > 1 => throw new ArmatureException(
                 $"More than one property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}."
               + $" Consider using a property name instead.'"),
        _ => new BuildResult(properties)
      };
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyByType)} {{ Type: {_type.ToLogString().QuoteIfNeeded()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}