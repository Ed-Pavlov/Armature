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

    var        buildResult = buildSession.BuildUnit(new UnitId(parameters, SpecialTag.Argument));
    object?[]? arguments   = null;

    if(buildResult.HasValue)
      arguments = (object?[]?) buildResult.Value;

    #region Error handling

    if(!buildResult.HasValue || arguments is null || arguments.Length != parameters.Length)
    {
      ArmatureException? exception = null;

      var message = "Arguments for method are not built";

      if(arguments is null)
        message += ": build result is 'null'";
      else if(arguments.Length != parameters.Length)
      {
        message   += ": built arguments count doesn't match parameters count";
        exception =  new ArmatureException(message).AddData("ArgumentsCount", arguments.Length);
      }

      exception ??= new ArmatureException(message);

      for(var i = 0; i < parameters.Length; i++)
      {
        var method = parameters[i].Member;

        exception.AddData($"Type#{i:00}", method.DeclaringType?.ToLogString())
                 .AddData($"Method#{i:00}", method)
                 .AddData($"Parameter#{i:00}", parameters[i]);
      }

      throw exception;
    }

    #endregion

    return arguments;
  }

  public static object? BuildArgumentForMethod(this IBuildSession buildSession, ParameterInfo parameter)
  {
    if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
    if(parameter is null) throw new ArgumentNullException(nameof(parameter));

    var buildResult = buildSession.BuildUnit(new UnitId(parameter, SpecialTag.Argument));

    if(!buildResult.HasValue)
    {
      var method = parameter.Member;

      throw new ArmatureException($"Argument for parameter '{parameter}' of {method.DeclaringType?.ToLogString()}.{method} is not built")
       .AddData("Method", method);
    }

    return buildResult.Value;
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
