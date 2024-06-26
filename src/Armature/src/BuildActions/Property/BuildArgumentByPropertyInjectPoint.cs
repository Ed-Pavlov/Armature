﻿using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Builds an argument for the property which is marked with <see cref="InjectAttribute"/> using <see cref="MemberInfo.MemberType"/> and
/// <see cref="InjectAttribute.Tag"/> as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByPropertyInjectPoint : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var propertyInfo = (PropertyInfo) buildSession.Stack.TargetUnit.Kind!;

    foreach(var attribute in propertyInfo.GetCustomAttributes<InjectAttribute>())
    {
      if(Log.IsEnabled(LogLevel.Trace))
        Log.WriteLine(LogLevel.Trace, $"Attribute: {attribute.ToHoconString()}");

      var unitInfo    = Unit.By(propertyInfo.PropertyType, attribute.Tag);
      var buildResult = buildSession.BuildUnit(unitInfo);

      if(buildResult.HasValue)
      {
        buildSession.BuildResult = buildResult;
        break;
      }
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildArgumentByPropertyInjectPoint);
}
