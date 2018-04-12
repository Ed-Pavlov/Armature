using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Common;
using Resharper.Annotations;

namespace Armature.Core
{
  public static class BuildSessionExtension
  {
    /// <summary>
    ///   Builds a <see cref="ConstructorInfo" /> for a <see creaf="type" /> by building a unit represented
    ///   by <see cref="UnitInfo" />(<see cref="type" />, <see cref="SpecialToken.Constructor" />) via current build session.
    /// </summary>
    public static ConstructorInfo GetConstructorOf([NotNull] this IBuildSession buildSessoin, [NotNull] Type type)
    {
      if (buildSessoin == null) throw new ArgumentNullException(nameof(buildSessoin));
      if (type == null) throw new ArgumentNullException(nameof(type));

      var result = buildSessoin.BuildUnit(new UnitInfo(type, SpecialToken.Constructor));
      if (result?.Value == null)
        throw new Exception(string.Format("Can't find appropriate constructor for type {0}", type));

      return (ConstructorInfo)result.Value;
    }

    public static IReadOnlyList<PropertyInfo> GetPropertiesToInject(this IBuildSession buildSession, Type type)
    {
      var unitInfo = new UnitInfo(type, SpecialToken.Property);
      var result = buildSession.BuildAllUnits(unitInfo);
      return result?.SelectMany(_ => (IReadOnlyList<PropertyInfo>)_.Value).ToArray() ?? EmptyArray<PropertyInfo>.Instance;
    }

    public static object GetValueForProperty(this IBuildSession buildSession, PropertyInfo propertyInfo)
    {
      var buildResult = buildSession.BuildUnit(new UnitInfo(propertyInfo, SpecialToken.InjectValue));
      return buildResult != null ? buildResult.Value : throw new ArmatureException(string.Format("Can't build value for property {0}", propertyInfo));
    }
    
    /// <summary>
    ///   Builds values for parameters by building a set of <see cref="UnitInfo" />(<see cref="parameters" />[i], <see cref="SpecialToken.InjectValue" />)
    ///   one by one via current build session
    /// </summary>
    public static object[] GetValuesForParameters([NotNull] this IBuildSession buildSession, [NotNull] ParameterInfo[] parameters)
    {
      if (buildSession == null) throw new ArgumentNullException(nameof(buildSession));
      if (parameters == null) throw new ArgumentNullException(nameof(parameters));
      if (parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", nameof(parameters));

      var values = new object[parameters.Length];
      for (var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.BuildUnit(new UnitInfo(parameters[i], SpecialToken.InjectValue));
        if (buildResult == null)
          throw new ArmatureException(string.Format("Can't build value for parameter {0}", parameters[i]));

        values[i] = buildResult.Value;
      }

      return values;
    }

    [DebuggerStepThrough]
    public static UnitInfo GetUnitUnderConstruction(this IBuildSession buildSessoin) => buildSessoin.BuildSequence.Last();
  }
}