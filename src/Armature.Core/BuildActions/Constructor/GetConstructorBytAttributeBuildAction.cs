using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Constructor
{
  /// <summary>
  /// "Builds" a constructor Unit of the currently building Unit marked with attribute which satisfies user provided conditions  
  /// </summary>
  public class GetConstructorBytAttributeBuildAction<T> : IBuildAction
  {
    private readonly Predicate<T> _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public GetConstructorBytAttributeBuildAction(Predicate<T> predicate = null) => _predicate = predicate;

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