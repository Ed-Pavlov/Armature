using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Armature.Core.Common;


namespace Armature.Core
{
  public static class BuildSessionExtension
  {
    /// <summary>
    ///   "Builds" a <see cref="ConstructorInfo" /> for a <see creaf="type" /> by building a unit represented
    ///   by <see cref="UnitId" />(<paramref name="type" />, <see cref="SpecialToken.Constructor" />) via current build session.
    /// </summary>
    public static ConstructorInfo GetConstructorOf(this IBuildSession buildSession, Type type)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var result = buildSession.BuildUnit(new UnitId(type, SpecialToken.Constructor));

      if(!result.HasValue)
        throw new Exception(string.Format("Can't find appropriate constructor for type {0}", type));

      return (ConstructorInfo) result.Value!;
    }

    /// <summary>
    ///   "Builds" a list of properties of currently building Unit (<paramref name="type" />) for injecting dependencies
    /// </summary>
    public static IReadOnlyList<PropertyInfo> GetPropertiesToInject(this IBuildSession buildSession, Type type)
    {
      var unitInfo = new UnitId(type, SpecialToken.Property);
      var result   = buildSession.BuildAllUnits(unitInfo);

      return result?.SelectMany(_ => (IReadOnlyList<PropertyInfo>) _.Value!).ToArray() ?? EmptyArray<PropertyInfo>.Instance;
    }

    /// <summary>
    ///   "Builds" a value to inject into the property representing by <paramref name="propertyInfo" />
    /// </summary>
    public static object? GetValueForProperty(this IBuildSession buildSession, PropertyInfo propertyInfo)
    {
      var buildResult = buildSession.BuildUnit(new UnitId(propertyInfo, SpecialToken.InjectValue));

      return buildResult.HasValue ? buildResult.Value : throw new ArmatureException(string.Format("Can't build value for property '{0}'", propertyInfo));
    }

    /// <summary>
    ///   "Builds" values for parameters by building a set of <see cref="UnitId" />(<paramref name="parameters" />[i], <see cref="SpecialToken.InjectValue" />)
    ///   one by one via current build session
    /// </summary>
    public static object?[] GetValuesForParameters(this IBuildSession buildSession, ParameterInfo[] parameters)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
      if(parameters is null) throw new ArgumentNullException(nameof(parameters));
      if(parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", nameof(parameters));

      var values = new object?[parameters.Length];

      for(var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.BuildUnit(new UnitId(parameters[i], SpecialToken.InjectValue));

        if(!buildResult.HasValue)
          throw new ArmatureException(string.Format("Can't build value for parameter '{0}'", parameters[i]));

        values[i] = buildResult.Value;
      }

      return values;
    }

    /// <summary>
    ///   Returns the currently building Unit in the build session
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnitId GetUnitUnderConstruction(this IBuildSession buildSession) => buildSession.BuildSequence.Last();
  }
}
