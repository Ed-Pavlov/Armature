using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Gets a constructor of type which matches specified parameter types list.
  /// </summary>
  public class GetConstructorByParameterTypes : IBuildAction
  {
    private readonly Type[] _parameterTypes;

    public GetConstructorByParameterTypes(params Type[] parameterTypes)
      => _parameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var ctor     = GetConstructor(unitType);

      if(ctor is not null)
        buildSession.BuildResult = new BuildResult(ctor);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    private ConstructorInfo? GetConstructor(Type unitType)
      => unitType.GetConstructors().FirstOrDefault(ctor => IsParametersListMatch(ctor.GetParameters(), _parameterTypes));

    private static bool IsParametersListMatch(ParameterInfo[] parameterInfos, Type[] parameterTypes)
    {
      if(parameterInfos.Length != parameterTypes.Length)
        return false;

      for(var i = 0; i < parameterInfos.Length; i++)
        if(parameterInfos[i].ParameterType != parameterTypes[i])
          return false;

      return true;
    }

    [DebuggerStepThrough]
    public override string ToString()
      => string.Format("{0}( {1} )", GetType().GetShortName(), string.Join(", ", _parameterTypes.Select(Log.ToLogString).ToArray()));
  }
}
