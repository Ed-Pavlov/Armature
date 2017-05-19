using System;
using System.Reflection;
using Armature.Common;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public static class UnitBuilderExtension
  {
    public static ConstructorInfo GetConstructorOf([NotNull] this UnitBuilder unitBuilder, [NotNull] Type type)
    {
      if (unitBuilder == null) throw new ArgumentNullException("unitBuilder");
      if (type == null) throw new ArgumentNullException("type");

      var result = unitBuilder.Build(new UnitInfo(type, SpecialToken.FindConstructor));
      if(result == null || result.Value == null)
        throw new Exception( string.Format("Can't find appropriate constructor for type {0}", type));
      return (ConstructorInfo) result.Value;
    }

    public static object[] GetValuesForParameters([NotNull] this UnitBuilder unitBuilder, [NotNull] ParameterInfo[] parameters)
    {
      if (unitBuilder == null) throw new ArgumentNullException("unitBuilder");
      if (parameters == null) throw new ArgumentNullException("parameters");
      if (parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", "parameters");

      var values = new object[parameters.Length];
      for (var i = 0; i < parameters.Length; i++)
      {
        var buildResult = unitBuilder.Build(new UnitInfo(parameters[i], SpecialToken.BuildParameterValue));
        if (buildResult == null)
          throw new ArmatureException("Can't build value for parameter").AddData("Parameter", parameters[i]);

        values[i] = buildResult.Value;
      }
      return values;
    }
  }
}