using System;
using System.Reflection;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public static class For
  {
    public static ParameterValueBuildPlanner Parameter<T>()
    {
      return new ParameterValueBuildPlanner(getBuildAction =>
        new StrictParameterTypeValueBuildStep(
          ParameterValueBuildActionWeight.TypedParameterResolver, 
          typeof(T), 
          getBuildAction));
    }

    public static ParameterValueBuildPlanner ParameterName(string parameterName)
    {
      return new ParameterValueBuildPlanner(getBuildAction =>
        new ParameterNameValueBuildStep(
          ParameterValueBuildActionWeight.NamedParameterResolver, 
          parameterName, 
          getBuildAction));
    }

    public static ParameterValueBuildPlanner ParameterId([CanBeNull] object injectPointId)
    {
      return new ParameterValueBuildPlanner(getBuildAction =>
        new AttributedParameterValueBuildStep(
          ParameterValueBuildActionWeight.AttributedParameterResolver, 
          injectPointId, 
          getBuildAction));
    }

    public class ParameterValueBuildPlanner : IParameterValueBuildPlanner
    {
      /// <summary>
      /// Factory method creates build step builds a value for a parameter, we can't pass it to constructor
      /// because does not have build action factory method, which appears later by calling <see cref="UseValue"/>, <see cref="UseToken"/> etc
      /// </summary>
      private readonly Func<Func<ParameterInfo, IBuildAction>, IBuildStep> _createBuildStep;
      
      /// <summary>
      /// Creates a build action based on passed by parameter value build step <see cref="ParameterInfo"/>,
      /// some strategies ignores it
      /// </summary>
      private Func<ParameterInfo, IBuildAction> _getBuildAction;

      public ParameterValueBuildPlanner(Func<Func<ParameterInfo, IBuildAction>, IBuildStep> createBuildStep)
      {
        _createBuildStep = createBuildStep;
      }

      public void AddBuildParameterValueStepTo(BuildStepBase buildStep)
      {
        if(_getBuildAction == null) throw new InvalidOperationException("Parameter value source not specified, did you forget call UseValue/UseToken etc?");
        buildStep.AddBuildStep(_createBuildStep(_getBuildAction));
      }
      
      public IParameterValueBuildPlanner UseValue([CanBeNull] object value)
      {
        // just return value for any parameter matched by build step
        _getBuildAction = parameterInfo =>
        {
          if (parameterInfo.ParameterType.IsInstanceOfType(value)) 
            return new SingletonBuildAction(value);
          
          var exception = new InvalidOperationException("The type of value provided for a parameter does not match with parameter type")
            .AddData("ParameterInfo", parameterInfo)
            .AddData("Value", value);
          if (value != null)
            exception.AddData("Value type", value.GetType());
          throw exception;
        }; 
        return this;
      }

      public IParameterValueBuildPlanner UseToken([NotNull] object token)
      {
        if (token == null) throw new ArgumentNullException("token");
        _getBuildAction = parameterInfo => new RedirectTypeBuildAction(parameterInfo.ParameterType, token); // build value by UnitInfo(parameterType, token)
        return this;
      }
    }
  }
}