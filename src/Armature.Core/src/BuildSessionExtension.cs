using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Armature.Core.Logging;


namespace Armature.Core
{
  public static class BuildSessionExtension
  {
    /// <summary>
    ///   Builds a <see cref="ConstructorInfo" /> for a <see creaf="type" /> by building a unit represented
    ///   by <see cref="UnitId" />(<paramref name="type" />, <see cref="SpecialKey.Constructor" />) via current build session.
    /// </summary>
    public static ConstructorInfo GetConstructorOf(this IBuildSession buildSession, Type type)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
      if(type is null) throw new ArgumentNullException(nameof(type));

      var result = buildSession.BuildUnit(new UnitId(type, SpecialKey.Constructor));

      if(!result.HasValue)
        throw new Exception(string.Format("Can't find appropriate constructor for type {0}", type));

      return (ConstructorInfo) result.Value!;
    }

    /// <summary>
    ///   Builds an argument to inject into the property representing by <paramref name="propertyInfo" />
    /// </summary>
    public static object? BuildArgument(this IBuildSession buildSession, PropertyInfo propertyInfo)
    {
      var buildResult = buildSession.BuildUnit(new UnitId(propertyInfo, SpecialKey.Argument));
      return buildResult.HasValue ? buildResult.Value : throw new ArmatureException(string.Format("Can't build value for property '{0}'", propertyInfo));
    }

    /// <summary>
    ///   "Builds" values for parameters by building a set of <see cref="UnitId" />(<paramref name="parameters" />[i], <see cref="SpecialKey.Argument" />)
    ///   one by one via current build session
    /// </summary>
    public static object?[] GetArgumentsForParameters(this IBuildSession buildSession, ParameterInfo[] parameters)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
      if(parameters is null) throw new ArgumentNullException(nameof(parameters));
      if(parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", nameof(parameters));

      using(Log.Block(LogLevel.Trace, () => "GetValuesForParameters( " + string.Join(", ", parameters.Select(_ => _.ToString()).ToArray()) + " )"))
      {
        var values = new object?[parameters.Length];

        for(var i = 0; i < parameters.Length; i++)
        {
          var buildResult = buildSession.BuildUnit(new UnitId(parameters[i], SpecialKey.Argument));

          if(!buildResult.HasValue)
            throw new ArmatureException(string.Format("Can't build value for parameter '{0}'", parameters[i]));

          values[i] = buildResult.Value;
        }

        return values;
      }
    }

    /// <summary>
    ///   Returns the currently building Unit in the build session
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnitId GetUnitUnderConstruction(this IBuildSession buildSession) => buildSession.BuildSequence.Last();
  }
}
