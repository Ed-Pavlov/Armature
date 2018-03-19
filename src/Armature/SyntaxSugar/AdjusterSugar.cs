using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Constructor;
using Armature.Framework.BuildActions.Property;
using Armature.Framework.UnitMatchers.Parameters;
using Armature.Framework.UnitMatchers.Properties;
using Armature.Framework.UnitSequenceMatcher;
using Armature.Interface;
using Armature.Properties;
using ConstructorMatcher = Armature.Framework.UnitMatchers.ConstructorMatcher;

namespace Armature
{
  public class AdjusterSugar : UnitSequenceExtensibility
  {

    [DebuggerStepThrough]
    public AdjusterSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {} 

    /// <summary>
    ///   The set of values for parameters of registered Unit can be values or implementation of <see cref="IBuildPlan" />.
    ///   See <see cref="ForParameter" /> for details
    /// </summary>
    public AdjusterSugar UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
      {
        if (parameter is ParameterValueBuildPlan buildPlan)
          ((IBuildPlan)buildPlan).Register(UnitSequenceMatcher);
        else if(parameter is IBuildPlan)
          throw new ArmatureException("IParameterValueBuildPlan or plain object value expected"); 
        else
          UnitSequenceMatcher
            .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(new ParameterByValueMatcher(parameter), InjectPointMatchingWeight.WeakTypedParameter))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(parameter));
      }

      return this;
    }

    public AdjusterSugar InjectProperty(params object[] values)
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Initialize, InjectIntoPropertiesBuildAction.Instance);
      
      foreach (var value in values)
      {
        if(value is IPropertyValueBuildPlan buildPlan)
          buildPlan.Register(UnitSequenceMatcher);
        else if(value is IBuildPlan)
          throw new ArmatureException("IPropertyValueBuildPlanor plain object value expected"); 
        else
          UnitSequenceMatcher
            .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(new PropertyByValueMatcher(value), InjectPointMatchingWeight.WeakTypedParameter))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(value));
      }
      
      return this;
    }

    /// <summary>
    ///   Register Unit as an eternal singleton <see cref="SingletonBuildAction" /> for details
    /// </summary>
    public void AsSingleton() => UnitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction());

    /// <summary>
    ///   Instantiate an Unit using a constructor marked with <see cref="InjectAttribute" />(<see cref="injectionPointId" />)
    /// </summary>
    public AdjusterSugar UsingInjectPointConstructor(object injectionPointId)
    {
      UnitSequenceMatcher
        .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance, 0))
        .AddBuildAction(BuildStage.Create, new GetInjectPointConstructorBuildAction(injectionPointId));
      return this;
    }

    public AdjusterSugar UsingParameterlessConstructor() => UsingConstructorWithParameters();

    public AdjusterSugar UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      UnitSequenceMatcher
        .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance, 0))
        .AddBuildAction(BuildStage.Create, new GetConstructorByParameterTypesBuildAction(parameterTypes));
      return this;
    }

    public AdjusterSugar BuildingWhich([NotNull] Action<BuildingSugar> tuneAction)
    {
      if (tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new BuildingSugar(UnitSequenceMatcher));
      return this;
    }
  }
}