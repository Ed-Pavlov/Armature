﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
///   "Builds" a constructor Unit of the currently building Unit with provided names
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
    var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();

    var properties =
      _names.Select(
               name =>
               {
                 var property = unitType.GetProperty(name);

                 if(property is null)
                   throw new ArmatureException(
                     string.Format("There is no property with name '{0}' in type '{1}'", name, unitType.ToLogString()));

                 return property;
               })
            .ToArray();

    buildSession.BuildResult = new BuildResult(properties);
  }

  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyListByNames)} {{ Names: {_names.ToHoconString()} }} }}";
}