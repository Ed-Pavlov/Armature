using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Parameter
{
  /// <summary>
  /// The simplest implementation of a resolving of an array of <see cref="ParameterInfo"/> usually needed to inject values into
  /// a constructor or a method. 
  /// </summary>
  [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
  public class CreateParametersArrayBuildAction : IBuildAction
  {
    public static readonly CreateParametersArrayBuildAction Instance = new();

    public void Process(IBuildSession buildSession)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));

      var parameters = (ParameterInfo[]) buildSession.GetUnitUnderConstruction().Id!;
      if(parameters.Length == 0) throw new ArgumentException("At least one parameters should be provided", nameof(parameters));

      var values = new object?[parameters.Length];
      for(var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.BuildUnit(CreateUnitInfoBy(parameters[i]));

        if(!buildResult.HasValue)
          throw new ArmatureException(string.Format("Can't build value for parameter '{0}'", parameters[i]));

        values[i] = buildResult.Value;
      }

      buildSession.BuildResult = new BuildResult(values);
    }

    public void PostProcess(IBuildSession buildSession) { }

    protected virtual UnitInfo CreateUnitInfoBy(ParameterInfo parameterInfo) => new(parameterInfo, SpecialToken.InjectValue);

    public override string ToString() => GetType().GetShortName();
  }
}
