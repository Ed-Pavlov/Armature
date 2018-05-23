using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;
using Resharper.Annotations;

namespace Armature.Core.BuildActions.Constructor
{
  /// <summary>
  ///   "Builds" a constructor Unit of the currently building Unit by parameter types
  /// </summary>
  public class GetConstructorByParameterTypesBuildAction : IBuildAction
  {
    private readonly Type[] _parameterTypes;

    public GetConstructorByParameterTypesBuildAction([NotNull] params Type[] parameterTypes) =>
      _parameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var ctor = GetConstructor(unitType);
      if (ctor != null)
        buildSession.BuildResult = new BuildResult(ctor);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    private ConstructorInfo GetConstructor([NotNull] Type unitType) =>
      unitType.GetConstructors().FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameterTypes));

    private static bool IsParametersListMatch(ParameterInfo[] parameterInfos, Type[] parameterTypes)
    {
      if (parameterInfos.Length != parameterTypes.Length)
        return false;

      for (var i = 0; i < parameterInfos.Length; i++)
        if (parameterInfos[i].ParameterType != parameterTypes[i])
          return false;

      return true;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(
      LogConst.OneParameterFormat,
      GetType().GetShortName(),
      string.Join(", ", _parameterTypes.Select(Log.AsLogString).ToArray()));
  }
}