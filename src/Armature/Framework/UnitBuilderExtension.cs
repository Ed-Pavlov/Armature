using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public static class UnitBuilderExtension
  {
    /// <summary>
    ///   Builds a <see cref="ConstructorInfo" /> for a <see creaf="type" /> by building a unit represented
    ///   by <see cref="UnitInfo" />(<see cref="type" />, <see cref="SpecialToken.Constructor" />) via current build session.
    /// </summary>
    public static ConstructorInfo GetConstructorOf([NotNull] this UnitBuilder unitBuilder, [NotNull] Type type)
    {
      if (unitBuilder == null) throw new ArgumentNullException(nameof(unitBuilder));
      if (type == null) throw new ArgumentNullException(nameof(type));

      var result = unitBuilder.Build(new UnitInfo(type, SpecialToken.Constructor));
      if (result == null || result.Value == null)
        throw new Exception(string.Format("Can't find appropriate constructor for type {0}", type));

      return (ConstructorInfo)result.Value;
    }

    /// <summary>
    ///   Builds values for parameters by building a set of <see cref="UnitInfo" />(<see cref="parameters" />[i], <see cref="SpecialToken.ParameterValue" />)
    ///   one by one via current build session
    /// </summary>
    public static object[] GetValuesForParameters([NotNull] this UnitBuilder unitBuilder, [NotNull] ParameterInfo[] parameters)
    {
      if (unitBuilder == null) throw new ArgumentNullException(nameof(unitBuilder));
      if (parameters == null) throw new ArgumentNullException(nameof(parameters));
      if (parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", nameof(parameters));

      var values = new object[parameters.Length];
      for (var i = 0; i < parameters.Length; i++)
      {
        var buildResult = unitBuilder.Build(new UnitInfo(parameters[i], SpecialToken.ParameterValue));
        if (buildResult == null)
          throw new ArmatureException(string.Format("Can't build value for parameter {0}", parameters[i]));

        values[i] = buildResult.Value;
      }

      return values;
    }

    [DebuggerStepThrough]
    public static UnitInfo GetUnitUnderConstruction(this UnitBuilder unitBuilder) => unitBuilder.BuildSequence.Last();
  }
}