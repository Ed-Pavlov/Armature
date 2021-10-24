using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Instantiates an object using reflection <see cref="ConstructorInfo.Invoke(object[])"/> method.
  /// </summary>
  public record CreateByReflection : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(!buildSession.BuildResult.HasValue)
      {
        var type = buildSession.GetUnitUnderConstruction().GetUnitType(); 
        
        Log.WriteLine(LogLevel.Verbose, $"Type = {type.ToLogString()}"  );

        if(!type.IsInterface && !type.IsAbstract)
        {
          var constructor = buildSession.GetConstructorOf(type);
          var parameters  = constructor.GetParameters();

          if(parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          object instance;

          try
          {
            if(parameters.Length == 0)
              instance = constructor.Invoke(Empty<object>.Array);
            else
            {
              var arguments = buildSession.BuildArgumentsForMethod(parameters);
              instance = constructor.Invoke(arguments);
            }
          }
          catch(TargetInvocationException e)
          {
            Console.WriteLine(e);
            throw;
          }

          buildSession.BuildResult = new BuildResult(instance);
        }
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
