using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class Tuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public Tuner(IPatternTreeNode parentNode) : base(parentNode) { }

    /// <summary>
    ///   Provides arguments to inject into building unit. See <see cref="ForParameter" /> for details.
    /// </summary>
    public Tuner UsingArguments(params object[] arguments) //TODO: надо как-то свести и Property и Methods к одному методу UsingArguments
    {
      if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

      foreach(var argument in arguments)
        if(argument is IArgumentTuner buildPlan)
          buildPlan.Apply(ParentNode);
        else if(argument is ITuner)
          throw new ArgumentException("IParameterValueBuildPlan or plain object value expected");
        else
          ParentNode
           .GetOrAddNode(new IfLastUnitMatches(new ParentTypePattern(argument.GetType(), null), InjectPointMatchingWeight.WeakTypedParameter))
           .UseBuildAction(new Value<object>(argument), BuildStage.Cache);

      return this;
    }

    /// <summary>
    ///   Provides arguments to inject into building units properties.   See <see cref="ForProperty" /> for details.
    ///   Also value can be a build plan returned by one of the method of the <see cref="Property" /> class,
    /// which specifies properties to inject dependencies.
    /// </summary>
    // public Tuner UsingArguments(params object[] arguments)
    // {
    //   ParentNode.UseBuildAction(InjectDependenciesIntoProperties.Instance, BuildStage.Initialize, true);
    //
    //   foreach(var argument in arguments)
    //     if(argument is IInjectionPointTuner buildPlan)
    //       buildPlan.Apply(ParentNode);
    //     else if(argument is ITuner)
    //       throw new ArmatureException("IPropertyValueBuildPlan or plain object value expected");
    //     else
    //       ParentNode
    //        .GetOrAddNode(new IfLastUnitMatches(new PropertyByTypePattern(argument.GetType(), false), InjectPointMatchingWeight.WeakTypedParameter))
    //        .UseBuildAction(new Singleton(argument), BuildStage.Create);
    //
    //   return this;
    // }

    public Tuner InjectInto(params IInjectPointTuner[] propertyIds)
    {
      ParentNode.UseBuildAction(InjectDependenciesIntoProperties.Instance, BuildStage.Initialize, true);

      foreach(var propertyId in propertyIds)
        propertyId.Apply(ParentNode);

      return this;
    }

    /// <summary>
    ///   Register Unit as an singleton with a lifetime equal to parent <see cref="BuildPlanCollection"/>. See <see cref="Singleton" /> for details
    /// </summary>
    public void AsSingleton() => ParentNode.UseBuildAction(new Singleton(), BuildStage.Cache);

    /// <summary>
    ///   Instantiate a Unit using a constructor with the biggest number of parameters
    /// </summary>
    public Tuner UsingLongestConstructor()
    {
      ParentNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(GetLongestConstructor.Instance, BuildStage.Create);

      return this;
    }

    /// <summary>
    ///   Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />)
    /// </summary>
    public Tuner UsingInjectPointConstructor(object? injectionPointId)
    {
      ParentNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(new GetConstructorByInjectPointId(injectionPointId), BuildStage.Create);

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
    ///   Instantiate a Unit using constructor with exact set of parameters specified in <paramref name="parameterTypes" />
    /// </summary>
    public Tuner UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      ParentNode
       .GetOrAddNode(new IfLastUnitMatches(ConstructorPattern.Instance))
       .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create);

      return this;
    }

    /// <summary>
    ///   Doing the same as <see cref="PatternTreeTunerExtension.Building{T}" /> but w/o breaking fluent syntax
    /// </summary>
    public Tuner BuildingWhich(Action<RootTuner> tuneAction)
    {
      if(tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new RootTuner(ParentNode));

      return this;
    }
  }
}
