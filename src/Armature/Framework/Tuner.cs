using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using Resharper.Annotations;
using ConstructorMatcher = Armature.Core.UnitMatchers.ConstructorMatcher;

namespace Armature
{
  public class Tuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public Tuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {} 

    /// <summary>
    ///   The set of values for parameters of registered Unit can be values or implementation of <see cref="IBuildPlan" />.
    ///   See <see cref="ForParameter" /> for details
    /// </summary>
    public Tuner UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
      {
        if (parameter is IParameterValueBuildPlan buildPlan)
          buildPlan.Register(UnitSequenceMatcher);
        else if(parameter is IBuildPlan)
          throw new ArmatureException("IParameterValueBuildPlan or plain object value expected"); 
        else
          UnitSequenceMatcher
            .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(new ParameterByValueMatcher(parameter), InjectPointMatchingWeight.WeakTypedParameter))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(parameter));
      }

      return this;
    }

    public Tuner InjectProperty(params object[] values)
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
            .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(new PropertyByValueMatcher(value), InjectPointMatchingWeight.WeakTypedParameter))
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
    public Tuner UsingInjectPointConstructor(object injectionPointId)
    {
      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetInjectPointConstructorBuildAction(injectionPointId));
      return this;
    }

    public Tuner UsingParameterlessConstructor() => UsingConstructorWithParameters();

    public Tuner UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetConstructorByParameterTypesBuildAction(parameterTypes));
      return this;
    }

    public Tuner BuildingWhich([NotNull] Action<SequenceTuner> tuneAction)
    {
      if (tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new SequenceTuner(UnitSequenceMatcher));
      return this;
    }
  }
}