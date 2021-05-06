using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

      // take all constructors
      var buildResultList = buildSession.BuildAllUnits(new UnitId(type, SpecialKey.Constructor));
      
      if(buildResultList.Count == 0)
        throw new Exception(string.Format("Can't find appropriate constructor for type {0}", type));

      // and choose one with the max matching weight
      return (ConstructorInfo)buildResultList.Max().Entity.Value!;
    }

    /// <summary>
    ///   Builds an argument to inject into the property representing by <paramref name="propertyInfo" />
    /// </summary>
    public static object? BuildPropertyArgument(this IBuildSession buildSession, PropertyInfo propertyInfo)
    {
      var buildResult = buildSession.BuildUnit(new UnitId(propertyInfo, SpecialKey.Argument));

      return buildResult.HasValue
               ? buildResult.Value
               : throw new ArmatureException(string.Format("Argument for property '{0}' of {1} is not built", propertyInfo, propertyInfo.DeclaringType));
    }

    /// <summary>
    ///   Returns the currently building Unit in the build session
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnitId GetUnitUnderConstruction(this IBuildSession buildSession) => buildSession.BuildSequence.Last();
  }
}
