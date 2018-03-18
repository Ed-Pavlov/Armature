using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class GeConstructorBytAttributeBuildAction<T> : IBuildAction
  {
    private readonly Predicate<T> _predicate;

    [DebuggerStepThrough]
    public GeConstructorBytAttributeBuildAction(Predicate<T> predicate = null) => _predicate = predicate;

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var ctor = GetConstructorInfo(unitType);
      if(ctor != null)
        buildSession.BuildResult = new BuildResult(ctor);
    }
      
    private ConstructorInfo GetConstructorInfo(Type unitType)
    {
      var constructorInfo = unitType
        .GetConstructors()
        .SingleOrDefault(
          ctor =>
            ctor
              .GetCustomAttributes(typeof(T), false)
              .OfType<T>()
              .SingleOrDefault(attribute => _predicate == null || _predicate(attribute)) != null);
      return constructorInfo;
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _predicate);
  }
}