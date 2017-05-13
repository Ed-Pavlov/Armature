using System;
using System.Reflection;
using Armature.Common;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public static class UnitBuilderExtension
  {
    public static ConstructorInfo GetConstructorOf([NotNull] this Build.Session buildSession, [NotNull] Type type)
    {
      if (buildSession == null) throw new ArgumentNullException("buildSession");
      if (type == null) throw new ArgumentNullException("type");

      var result = buildSession.Build(new UnitInfo(type, SpecialToken.FindConstructor));
      if(result == null || result.Value == null)
        throw new Exception( string.Format("Can't find appropriate constructor for type {0}", type));
      return (ConstructorInfo) result.Value;
    }

    public static object[] GetValuesForParameters([NotNull] this Build.Session buildSession, [NotNull] ParameterInfo[] parameters)
    {
      if (buildSession == null) throw new ArgumentNullException("buildSession");
      if (parameters == null) throw new ArgumentNullException("parameters");
      if (parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", "parameters");

      var values = new object[parameters.Length];
      for (var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.Build(new UnitInfo(parameters[i], SpecialToken.BuildParameterValue));
        if (buildResult == null)
          throw new ArmatureException("Can't build value for parameter").AddData("Parameter", parameters[i]);

        values[i] = buildResult.Value;
      }
      return values;
    }
  }
}