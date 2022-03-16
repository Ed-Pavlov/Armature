using System;
using System.Reflection;

namespace Armature.Core.Sdk;

public static class BuildSessionExtension
{
  /// <summary>
  /// Builds a <see cref="ConstructorInfo" /> for a <see creaf="type" /> by building a unit represented
  /// by <see cref="UnitId" />(<paramref name="type" />, <see cref="SpecialTag.Constructor" />) via current build session.
  /// </summary>
  public static ConstructorInfo GetConstructorOf(this IBuildSession buildSession, Type type)
  {
    if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
    if(type is null) throw new ArgumentNullException(nameof(type));

    var result = buildSession.BuildUnit(new UnitId(type, SpecialTag.Constructor));

    if(!result.HasValue)
      throw new ArmatureException($"Constructor for type {type} is not found, check registrations for this type or 'default' registrations.");

    return (ConstructorInfo) result.Value!;
  }

  public static object?[] BuildArgumentsForMethod(this IBuildSession buildSession, ParameterInfo[] parameters)
  {
    if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
    if(parameters is null) throw new ArgumentNullException(nameof(parameters));
    if(parameters.Length == 0) throw new ArgumentException("At least one parameter should be provided", nameof(parameters));

    return (object?[]) buildSession.BuildUnit(new UnitId(parameters, SpecialTag.Argument)).Value!;
  }

  /// <summary>
  /// Builds an argument to inject into the property representing by <paramref name="propertyInfo" />
  /// </summary>
  public static object? BuildPropertyArgument(this IBuildSession buildSession, PropertyInfo propertyInfo)
  {
    var buildResult = buildSession.BuildUnit(new UnitId(propertyInfo, SpecialTag.Argument));

    return buildResult.HasValue
             ? buildResult.Value
             : throw new ArmatureException(string.Format("Argument for property '{0}' of {1} is not built", propertyInfo, propertyInfo.DeclaringType));
  }
}