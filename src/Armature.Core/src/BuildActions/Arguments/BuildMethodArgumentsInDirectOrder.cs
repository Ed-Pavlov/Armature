using System;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Builds arguments for method parameters by building a unit {<see cref="ParameterInfo"/>, <see cref="SpecialKey.Argument"/> one by one. 
  /// </summary>
  public record BuildMethodArgumentsInDirectOrder : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(buildSession is null) throw new ArgumentNullException(nameof(buildSession));
      Log.WriteLine(LogLevel.Verbose, "");

      var parameters = (ParameterInfo[])buildSession.GetUnitUnderConstruction().Kind!;
      var arguments = new object?[parameters.Length];

      for(var i = 0; i < parameters.Length; i++)
      {
        var buildResult = buildSession.BuildUnit(new UnitId(parameters[i], SpecialKey.Argument));
        
        if(!buildResult.HasValue)
        {
          var method = parameters[i].Member;
          throw new ArmatureException(string.Format("Argument for parameter '{0}' of {1} {2} is not built", parameters[i], method.DeclaringType, method))
           .AddData("Method", method);
        }

        arguments[i] = buildResult.Value;
      }

      buildSession.BuildResult = new BuildResult(arguments);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
