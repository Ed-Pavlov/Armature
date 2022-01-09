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
public record GetPropertyByType(Type _type) : IBuildAction, ILogString
{
  private readonly Type _type = _type ?? throw new ArgumentNullException(nameof(_type));

  public void Process(IBuildSession buildSession)
  {
    var unitType   = buildSession.BuildChain.TargetUnit.GetUnitType();
    var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();

    if(properties.Length > 0)
      buildSession.BuildResult = new BuildResult(properties);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyByType)} {{ Type: {_type.ToLogString().QuoteIfNeeded()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}