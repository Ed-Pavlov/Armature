using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Creates a Unit using reflection.
  /// </summary>
  public class CreateByReflection : IBuildAction
  {
    public static readonly IBuildAction Instance = new CreateByReflection();

    private CreateByReflection() { }

    public void Process(IBuildSession buildSession)
    {
      if(!buildSession.BuildResult.HasValue)
      {
        var type = buildSession.GetUnitUnderConstruction().GetUnitType();

        // ReSharper disable once PossibleNullReferenceException
        if(!type.IsInterface && !type.IsAbstract)
        {
          var constructor = buildSession.GetConstructorOf(type);
          var parameters = constructor.GetParameters();

          if(parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          try
          {
            object instance;

            if(parameters.Length == 0)
              instance = constructor.Invoke(Empty<object>.Array);
            else
            {
              var arguments = buildSession.GetArgumentsForParameters(parameters);
              instance = constructor.Invoke(arguments);
            }

            buildSession.BuildResult = new BuildResult(instance);
          }
          catch(TargetInvocationException exception)
          {
            if(exception.InnerException is null) throw;

            // extract original exception from TargetInvocationException and throw it,
            // store original stack trace as exception data since throwing will replace it
            exception.InnerException.AddData(ExceptionData.TargetInvocationStackTrace, exception.StackTrace);
            exception.InnerException.AddData(ExceptionData.OriginalStackTrace, exception.InnerException.StackTrace);

            throw exception.InnerException;
          }
        }
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
