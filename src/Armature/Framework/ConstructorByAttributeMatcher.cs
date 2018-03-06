using System;
using System.Linq;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class ConstructorByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;
    private readonly IBuildAction _buildAction;

    public ConstructorByAttributeMatcher(Predicate<T> predicate = null)
    {
      _predicate = predicate;
      _buildAction = new BuildActionImpl(this);
    }

    public bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      if (unitType == null || unitInfo.Token != SpecialToken.Constructor)
        return false;

      var constructorInfo = GetConstructorInfo(unitType);
      return constructorInfo != null;
    }

    private ConstructorInfo GetConstructorInfo(Type unitType)
    {
      var constructorInfo = unitType
        .GetConstructors()
        .SingleOrDefault(ctor =>
          ctor
            .GetCustomAttributes(typeof(T), false)
            .OfType<T>()
            .SingleOrDefault(attribute => _predicate == null || _predicate(attribute)) != null);
      return constructorInfo;
    }

    public bool Equals(IUnitMatcher other)
    {
      var matcher = other as ConstructorByAttributeMatcher<T>;
      return matcher != null && Equals(_predicate, matcher._predicate);
    }

    public IBuildAction BuildAction
    {
      get { return _buildAction; }
    }

    private class BuildActionImpl : IBuildAction
    {
      private readonly ConstructorByAttributeMatcher<T> _owner;

      public BuildActionImpl(ConstructorByAttributeMatcher<T> owner)
      {
        _owner = owner;
      }

      public void Process(UnitBuilder unitBuilder)
      {
        var unitType = unitBuilder.GetUnitUnderConstruction().GetUnitType();
        var constructorInfo = _owner.GetConstructorInfo(unitType);
        unitBuilder.BuildResult = new BuildResult(constructorInfo);
      }

      public void PostProcess(UnitBuilder unitBuilder)
      {
      }
    }
  }
}