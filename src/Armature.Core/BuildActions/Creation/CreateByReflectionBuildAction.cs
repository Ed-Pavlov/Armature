using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Creation
{
  /// <summary>
  ///   Builds an Unit using reflection
  /// </summary>
  public class CreateByReflectionBuildAction : IBuildAction
  {
    // it has no state, so use a singleton
    public static readonly IBuildAction Instance = new CreateByReflectionBuildAction();

    private CreateByReflectionBuildAction() { }

    public void Process(IBuildSession buildSession)
    {
      if (buildSession.BuildResult == null)
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
            object instance;
            if (parameters.Length == 0)
            {
              instance = constructor.Invoke(EmptyArray<object>.Instance);
            }
            else
            {
              object[] valuesForParameters;
              using (Log.Block(LogLevel.Trace, "Looking for parameters"))
              {
                valuesForParameters = buildSession.GetValuesForParameters(parameters);
              }

              instance = constructor.Invoke(valuesForParameters);
            }

            buildSession.BuildResult = new BuildResult(instance);
          }
          catch (TargetInvocationException exception)
          {
            if (exception.InnerException != null)
              throw new Exception("", exception.InnerException);

            throw;
          }
        }
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}