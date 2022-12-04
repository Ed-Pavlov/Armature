using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// "Builds" a list of properties of the Target Unit with give names
/// </summary>
public record GetPropertyListByNames : IBuildAction, ILogString
{
  private readonly IReadOnlyCollection<string> _names;

  public GetPropertyListByNames(params string[] names)
  {
    if(names is null) throw new ArgumentNullException(nameof(names));
    if(names.Length == 0) throw new ArgumentException("Specify property name", nameof(names));

    _names = names;
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();

    var properties = _names.Select(name => unitType.GetProperty(name)).Where(_ => _ is not null).ToArray();
    buildSession.BuildResult = new BuildResult(properties);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyListByNames)} {{ Names: {_names.ToHoconString()} }} }}";
}