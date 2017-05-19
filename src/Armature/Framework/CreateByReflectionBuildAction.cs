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

    public void Execute(UnitBuilder unitBuilder)
    {
      if(unitBuilder.BuildResult == null)
			{
        var type = unitBuilder.UnitInfo.GetUnitType();

			  // ReSharper disable once PossibleNullReferenceException
        if( !type.IsInterface && !type.IsAbstract )
        {
          var constructor = unitBuilder.GetConstructorOf(type);
          var parameters = constructor.GetParameters();

          if (parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          try
          {
            var instance = parameters.Length == 0 
              ? Activator.CreateInstance(type) 
              : Activator.CreateInstance(type, unitBuilder.GetValuesForParameters(parameters));
            unitBuilder.BuildResult = new BuildResult(instance);
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

    public void PostProcess(UnitBuilder unitBuilder)
    {}

    public override string ToString()
    {
      return GetType().Name;
    }
  }
}