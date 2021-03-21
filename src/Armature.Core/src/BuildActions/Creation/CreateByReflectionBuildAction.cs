using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Creation
{
  /// <summary>
  ///   Builds a Unit using reflection
  /// </summary>
  public class CreateByReflectionBuildAction : IBuildAction
  {
    // it has no state, so use a singleton
    public static readonly IBuildAction Instance = new CreateByReflectionBuildAction();

    private CreateByReflectionBuildAction() { }

    public void Process(IBuildSession buildSession)
    {
      if (buildSession.BuildResult is null)
      {
        var type = buildSession.GetUnitUnderConstruction().GetUnitType();

        // ReSharper disable once PossibleNullReferenceException
        if (!type.IsInterface && !type.IsAbstract)
        {
          ConstructorInfo constructor;
          using (Log.Block(LogLevel.Trace, "Looking for constructor"))
          {
            constructor = buildSession.GetConstructorOf(type);
          }

          var parameters = constructor.GetParameters();

          if (parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          try
          {
            BuildResult buildResult;
            if (parameters.Length == 0)
            {
              buildResult = new BuildResult(constructor.Invoke(EmptyArray<object>.Instance));
            }
            else
            {
              BuildResult[] values;
              using (Log.Block(LogLevel.Trace, () => "Looking for parameters [" + string.Join(", ", parameters.Select(_ => _.ToString()).ToArray()) + "]"))
              {
                values = buildSession.BuildValuesForParameters(parameters);
              }

              buildResult = new BuildResult(() => constructor.Invoke(values.GetResolvedValues()));
            }

            buildSession.BuildResult = buildResult;
          }
          catch (TargetInvocationException exception)
          {
            if (exception.InnerException == null) throw;

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