using System;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Builds arguments for method parameters by building a unit {<see cref="ParameterInfo"/>, <see cref="SpecialKey.Argument"/> one by one. 
  /// </summary>
  public class BuildMethodArguments : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));

      var method     = (MethodBase)buildSession.GetUnitUnderConstruction().Kind!;
      var parameters = method.GetParameters();

      var arguments = new object?[parameters.Length];

      for(var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.BuildUnit(new UnitId(parameters[i], SpecialKey.Argument));

        if(!buildResult.HasValue)
          throw new ArmatureException(string.Format("Argument for parameter '{0}' of {1} {2} is not built", parameters[i], method.DeclaringType, method))
           .AddData("Method", method.ToString());

        arguments[i] = buildResult.Value;
      }

      buildSession.BuildResult = new BuildResult(arguments);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
