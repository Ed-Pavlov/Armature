using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class Tuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public Tuner(IPatternTreeNode patternTreeNode) : base(patternTreeNode) { }

    /// <summary>
    ///   Provided values will be used to inject the into created object. See <see cref="ForParameter" /> for details
    /// </summary>
    public Tuner UsingArguments(params object[] values)
    {
      if(values is null || values.Length == 0)
        throw new Exception("null");

      foreach(var parameter in values)
        if(parameter is IParameterValueBuildPlan buildPlan)
          buildPlan.Apply(PatternTreeNode);
        else if(parameter is IBuildPlan)
          throw new ArmatureException("IParameterValueBuildPlan or plain object value expected");
        else
          PatternTreeNode
           .GetOrAddNode(new IfLastUnitMatches(new MethodParameterByTypePattern(parameter.GetType(), false), InjectPointMatchingWeight.WeakTypedParameter))
           .UseBuildAction(BuildStage.Create, new Singleton(parameter));

      return this;
    }

    /// <summary>
    ///   Provided values will be injected into properties of the created object.  See <see cref="ForProperty" /> for details.
    ///   Also value can be a build plan returned by one of the method of the <see cref="Property" /> class, which specifies properties to inject dependencies.
    /// </summary>
    public Tuner InjectProperty(params object[] values)
    {
      PatternTreeNode.UseBuildAction(BuildStage.Initialize, InjectIntoProperties.Instance);

      foreach(var value in values)
        if(value is IPropertyValueBuildPlan buildPlan)
          buildPlan.Apply(PatternTreeNode);
        else if(value is IBuildPlan)
          throw new ArmatureException("IPropertyValueBuildPlan or plain object value expected");
        else
          PatternTreeNode
           .GetOrAddNode(new IfLastUnitMatches(new PropertyByTypePattern(value.GetType(), false), InjectPointMatchingWeight.WeakTypedParameter))
           .UseBuildAction(BuildStage.Create, new Singleton(value));

      return this;
    }

    /// <summary>
    ///   Register Unit as an eternal singleton <see cref="Singleton" /> for details
    /// </summary>
    public void AsSingleton() => PatternTreeNode.UseBuildAction(BuildStage.Cache, new Singleton());

    /// <summary>
    ///   Instantiate a Unit using a constructor with the biggest number of parameters
    /// </summary>
    public Tuner UsingLongestConstructor()
    {
      PatternTreeNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(BuildStage.Create, GetLongestConstructor.Instance);

      return this;
    }

    /// <summary>
    ///   Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />)
    /// </summary>
    public Tuner UsingInjectPointConstructor(object injectionPointId)
    {
      PatternTreeNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(BuildStage.Create, new GetConstructorByInjectPointId(injectionPointId));

      return this;
    }

    /// <summary>
    ///   Instantiate a Unit using constructor without parameters
    /// </summary>
    public Tuner UsingParameterlessConstructor() => UsingConstructorWithParameters();

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1>() => UsingConstructorWithParameters(typeof(T1));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2>() => UsingConstructorWithParameters(typeof(T1), typeof(T2));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2, T3>() => UsingConstructorWithParameters(typeof(T1), typeof(T2), typeof(T3));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2, T3, T4>() => UsingConstructorWithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters as provided in <paramref name="parameterTypes" />
    /// </summary>
    public Tuner UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      PatternTreeNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(BuildStage.Create, new GetConstructorByParameterTypes(parameterTypes));

      return this;
    }

    /// <summary>
    ///   Doing the same as <see cref="PatternTreeTunerExtension.Building{T}" /> but w/o breaking fluent syntax
    /// </summary>
    public Tuner BuildingWhich(Action<SequenceTuner> tuneAction)
    {
      if(tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new SequenceTuner(PatternTreeNode));

      return this;
    }
  }
}
