using System;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class CreateByReflectionBuildAction : IBuildAction
  {
    // it has no state, so use a singleton
    public static readonly IBuildAction Instance = new CreateByReflectionBuildAction();

    private CreateByReflectionBuildAction()
    {}

    public void Execute(Build.Session buildSession)
    {
      if(buildSession.BuildResult == null)
			{
        var type = buildSession.UnitInfo.GetUnitType();

			  // ReSharper disable once PossibleNullReferenceException
        if( !type.IsInterface && !type.IsAbstract )
        {
          var constructor = buildSession.GetConstructorOf(type);
          var parameters = constructor.GetParameters();

          if (parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          try
          {
            var instance = parameters.Length == 0 
              ? Activator.CreateInstance(type) 
              : Activator.CreateInstance(type, buildSession.GetValuesForParameters(parameters));
            buildSession.BuildResult = new BuildResult(instance);
          }
          catch (TargetInvocationException exception)
          {
            if(exception.InnerException != null)
              throw new Exception("", exception.InnerException);
            throw;
          }
        }
			}
    }

    public void PostProcess(Build.Session buildSession)
    {}
  }
}