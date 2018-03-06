using System;
using System.Linq;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  /// <inheritdoc />
  /// <summary>
  /// Build action instantiates an object of type <see cref="UnitInfo.Id" /> as Type using reflection
  /// </summary>
  public class CreateByReflectionBuildAction : IBuildAction
  {
    // it has no state, so use a singleton
    public static readonly IBuildAction Instance = new CreateByReflectionBuildAction();

    private CreateByReflectionBuildAction()
    {}

    public void Process(UnitBuilder unitBuilder)
    {
      if(unitBuilder.BuildResult == null)
			{
        var type = unitBuilder.GetUnitUnderConstruction().GetUnitType();

			  // ReSharper disable once PossibleNullReferenceException
        if( !type.IsInterface && !type.IsAbstract )
        {
          var constructor = unitBuilder.GetConstructorOf(type);
          var parameters = constructor.GetParameters();

          if (parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
            return;

          try
          {
            object instance;
            if (parameters.Length == 0)
              instance = Activator.CreateInstance(type);
            else
            {
              var valuesForParameters = unitBuilder.GetValuesForParameters(parameters);
              instance = Activator.CreateInstance(type, valuesForParameters);
            }
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