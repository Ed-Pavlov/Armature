using System;
using System.Diagnostics;
using System.Linq;
using Armature.Core;
using Armature.Core.Annotations;
using Armature.Sdk;

namespace Armature.BuildActions.Property;

/// <summary>
/// Gets a property by specified type.
/// </summary>
public record GetPropertyByType(Type _type) : IBuildAction, ILogString
{
  private readonly Type _type = _type ?? throw new ArgumentNullException(nameof(_type));

  public void Process(IBuildSession buildSession)
  {
    var unitType   = buildSession.Stack.TargetUnit.GetUnitType();
    var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();

    switch(properties.Length)
    {
      case > 1:
        throw new ArmatureException($"More than one property with type {_type.GetFullName()} are found. It's ambiguous which one to inject dependency into.")
         .AddData("PropertyType", _type.GetFullName());

      case > 0: buildSession.BuildResult = new BuildResult(properties);
        break;
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyByType)} {{ Type: {_type.ToLogString().QuoteIfNeeded()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}