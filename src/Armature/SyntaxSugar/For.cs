using System;
using System.Reflection;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public static class For
  {
    /// <summary>
    /// Matches with parameter with <see cref="ParameterInfo.ParameterType"/> equals to <see cref="T"/>
    /// </summary>
    public static ParameterValueBuildPlanner Parameter<T>()
    {
      return new ParameterValueBuildPlanner(getBuildAction =>
        new StrictParameterTypeValueBuildStep(
          ParameterValueBuildActionWeight.TypedParameterResolver, 
          typeof(T), 
          getBuildAction));
    }

    /// <summary>
    /// Matches with parameter with <see cref="ParameterInfo.Name"/> equals to <see cref="parameterName"/>
    /// </summary>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    public static ParameterValueBuildPlanner ParameterName(string parameterName)
    {
      return new ParameterValueBuildPlanner(getBuildAction =>
        new NamedParameterValueBuildStep(
          ParameterValueBuildActionWeight.NamedParameterResolver, 
          parameterName, 
          getBuildAction));
    }

    /// <summary>
    /// Matches with parameter marked with <see cref="InjectAttribute"/>(<see cref="injectPointId"/>)
    /// </summary>
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

      void IParameterValueBuildPlanner.AddBuildParameterValueStepTo(BuildStepBase buildStep)
      {
        if(_getBuildAction == null) throw new InvalidOperationException("Parameter value source not specified, did you forget call UseValue/UseToken etc?");
        buildStep.AddBuildStep(_createBuildStep(_getBuildAction));
      }

      /// <summary>
      /// Use the <see cref="value"/> for the parameter
      /// </summary>
      public IParameterValueBuildPlanner UseValue([CanBeNull] object value)
      {
        // just return value for any parameter matched by build step
        _getBuildAction = parameterInfo =>
        {
          if (value == null || parameterInfo.ParameterType.IsInstanceOfType(value)) 
            return new SingletonBuildAction(value);

          throw new InvalidOperationException("The type of value provided for a parameter does not match with parameter type")
            .AddData("ParameterInfo", parameterInfo)
            .AddData("Value", value)
            .AddData("Value type", value.GetType());
        }; 
        return this;
      }

      /// <summary>
      /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType"/> and <see cref="token"/>
      /// </summary>
      public IParameterValueBuildPlanner UseToken([NotNull] object token)
      {
        if (token == null) throw new ArgumentNullException("token");
        _getBuildAction = parameterInfo => new RedirectTypeBuildAction(parameterInfo.ParameterType, token); // build value by UnitInfo(parameterType, token)
        return this;
      }

      public IParameterValueBuildPlanner UseResolver<T>(Func<UnitBuilder, T, object> resolver)
      {
        _getBuildAction = _ => new CreateWithFactoryMethodBuildAction<T, object>(resolver);
        return this;
      }
    }
  }
}