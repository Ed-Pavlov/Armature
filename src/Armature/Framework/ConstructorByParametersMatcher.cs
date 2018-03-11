using System;
using System.Diagnostics;
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

    [DebuggerStepThrough]
    public ConstructorByParametersMatcher([NotNull] params Type[] parameters)
    {
      if (parameters == null) throw new ArgumentNullException(nameof(parameters));

      _parameters = parameters;
      BuildAction = new BuildActionImpl(this);
    }

    public IBuildAction BuildAction { get; }

    public bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType != null && GetConstructor(unitType) != null;
    }

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj) => obj is ConstructorByParametersMatcher other && _parameters.EqualsTo(other._parameters);

    private ConstructorInfo GetConstructor([NotNull] Type unitType) => unitType.GetConstructors().FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameters));

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

      [DebuggerStepThrough]
      public BuildActionImpl(ConstructorByParametersMatcher owner) => _owner = owner;

      public void Process(UnitBuilder unitBuilder)
      {
        var unitType = unitBuilder.GetUnitUnderConstruction().GetUnitType();
        var constructor = _owner.GetConstructor(unitType);
        unitBuilder.BuildResult = new BuildResult(constructor);
      }

      [DebuggerStepThrough]
      public void PostProcess(UnitBuilder unitBuilder) { }
    }
  }
}