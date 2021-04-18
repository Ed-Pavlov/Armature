using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Gets a constructor of type marked with attribute which satisfies user provided conditions.
  /// </summary>
  public class GetConstructorBytAttribute<T> : IBuildAction
  {
    private readonly Predicate<T>? _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public GetConstructorBytAttribute(Predicate<T>? predicate = null) => _predicate = predicate;

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var ctor     = GetConstructorInfo(unitType);

      if(ctor is not null)
        buildSession.BuildResult = new BuildResult(ctor);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    private ConstructorInfo? GetConstructorInfo(Type unitType)
    {
      var constructorInfo = unitType
                           .GetConstructors()
                           .SingleOrDefault(
                              ctor =>
                                ctor
                                 .GetCustomAttributes(typeof(T), false)
                                 .OfType<T>()
                                 .SingleOrDefault(attribute => _predicate is null || _predicate(attribute)) is not null);

      return constructorInfo;
    }

    public override string ToString() => GetType().GetShortName();
  }
}
