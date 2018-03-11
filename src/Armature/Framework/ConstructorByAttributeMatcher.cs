using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class ConstructorByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;

    [DebuggerStepThrough]
    public ConstructorByAttributeMatcher(Predicate<T> predicate = null)
    {
      _predicate = predicate;
      BuildAction = new BuildActionImpl(this);
    }

    public IBuildAction BuildAction { get; }

    public bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      if (unitType == null || unitInfo.Token != SpecialToken.Constructor)
        return false;

      var constructorInfo = GetConstructorInfo(unitType);
      return constructorInfo != null;
    }

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ConstructorByAttributeMatcher<T> matcher && Equals(_predicate, matcher._predicate);

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

    private class BuildActionImpl : IBuildAction
    {
      private readonly ConstructorByAttributeMatcher<T> _owner;

      [DebuggerStepThrough]
      public BuildActionImpl(ConstructorByAttributeMatcher<T> owner) => _owner = owner;

      public void Process(UnitBuilder unitBuilder)
      {
        var unitType = unitBuilder.GetUnitUnderConstruction().GetUnitType();
        var constructorInfo = _owner.GetConstructorInfo(unitType);
        unitBuilder.BuildResult = new BuildResult(constructorInfo);
      }

      [DebuggerStepThrough]
      public void PostProcess(UnitBuilder unitBuilder) { }
    }
  }
}