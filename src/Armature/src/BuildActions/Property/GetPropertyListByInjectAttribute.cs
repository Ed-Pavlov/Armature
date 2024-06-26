﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Gets a list of  properties marked with <see cref="InjectAttribute" /> with specified tags <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />.
/// </summary>
public record GetPropertyListByInjectAttribute : IBuildAction, ILogString
{
  private readonly object?[] _tags;

  public GetPropertyListByInjectAttribute(params object?[] tags) => _tags = tags ?? throw new ArgumentNullException(nameof(tags));

  public void Process(IBuildSession buildSession)
  {
    var type = buildSession.Stack.TargetUnit.GetUnitType();

    var propertiesWithAttributes =
      type.GetProperties()
          .Select(
             property =>
             {
               var attributes = property.GetCustomAttributes<InjectAttribute>();
               return Tuple.Create(attributes, property);
             })
          .Where(_ => _.Item1.Any())
          .ToArray();

    var properties =
      (_tags.Length == 0
         ? propertiesWithAttributes.Select(_ => _.Item2)
         : _tags.SelectMany(tag => propertiesWithAttributes.Where(_ => _.Item1.Any(item => Equals(tag, item.Tag))).Select(_ => _.Item2))
                    .Distinct())
     .ToArray();

    if(properties.Length > 0)
      buildSession.BuildResult = new BuildResult(properties);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetPropertyListByInjectAttribute)} {{ Points: {_tags.ToHoconString()} }} }}";
}
