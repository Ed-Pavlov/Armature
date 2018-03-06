using System;
using System.Linq;
using System.Reflection;
using Armature.Common;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ConstructorByParametersMatcher : IUnitMatcher
  {
    private readonly Type[] _parameters;
    private readonly IBuildAction _buildAction;

    public ConstructorByParametersMatcher([NotNull] params Type[] parameters)
    {
      if (parameters == null) throw new ArgumentNullException("parameters");
      _parameters = parameters;
      _buildAction = new BuildActionImpl(this);
    }

    public IBuildAction BuildAction
    {
      get { return _buildAction; }
    }

    public bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType != null && GetConstructor(unitType) != null;
    }

    public bool Equals(IUnitMatcher obj)
    {
      var other = obj as ConstructorByParametersMatcher;
      return other != null && _parameters.EqualsTo(other._parameters);
    }

    private ConstructorInfo GetConstructor([NotNull] Type unitType)
    {
      return unitType.GetConstructors().FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameters));
    }

    private static bool IsParametersListMatch(ParameterInfo[] parameterInfos, Type[] parameterTypes)
    {
      if (parameterInfos.Length != parameterTypes.Length)
        return false;

      for (var i = 0; i < parameterInfos.Length; i++)
        if (parameterInfos[i].ParameterType != parameterTypes[i])
          return false;
      return true;
    }
    
    private class BuildActionImpl : IBuildAction
    {
      private readonly ConstructorByParametersMatcher _owner;

      public BuildActionImpl(ConstructorByParametersMatcher owner)
      {
        _owner = owner;
      }

      public void Process(UnitBuilder unitBuilder)
      {
        var unitType = unitBuilder.GetUnitUnderConstruction().GetUnitType();
        var constructor = _owner.GetConstructor(unitType);
        unitBuilder.BuildResult = new BuildResult(constructor);
      }

      public void PostProcess(UnitBuilder unitBuilder)
      {
      }
    }
  }
}