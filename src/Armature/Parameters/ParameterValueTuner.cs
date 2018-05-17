using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.BuildActions.Parameter;
using Resharper.Annotations;

namespace Armature.Parameters
{
  public class ParameterValueTuner
  {
    private readonly IUnitMatcher _parameterMatcher;
    private readonly int _weight;

    [DebuggerStepThrough]
    public ParameterValueTuner([NotNull] IUnitMatcher parameterMatcher, int weight)
    {
      _parameterMatcher = parameterMatcher ?? throw new ArgumentNullException(nameof(parameterMatcher));
      _weight = weight;
    }

    /// <summary>
    ///   Use the <paramref name="value"/> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue([CanBeNull] object value) => new ParameterValueBuildPlan(_parameterMatcher, new SingletonBuildAction(value), _weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="token"/>
    /// </summary>
    public ParameterValueBuildPlan UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));
      return new ParameterValueBuildPlan(_parameterMatcher, new CreateParameterValueBuildAction(token), _weight);
    }

    /// <summary>
    /// For building a value for the parameter use the type of parameter and <see cref="InjectAttribute.InjectionPointId"/> as token
    /// </summary>
    public ParameterValueBuildPlan UseInjectPointIdAsToken() => new ParameterValueBuildPlan(
      _parameterMatcher,
      CreateParameterValueForInjectPointBuildAction.Instance,
      _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T>(Func<IBuildSession, T, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T, object>(factoryMethod), _weight);
    
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2>(Func<IBuildSession, T1, T2, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, object>(factoryMethod), _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3>(Func<IBuildSession, T1, T2, T3, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4>(Func<IBuildSession, T1, T2, T3, T4, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5>(Func<IBuildSession, T1, T2, T3, T4, T5, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<IBuildSession, T1, T2, T3, T4, T5, T6, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), _weight);
    
    /// <summary>
    /// For building a value for the parameter use <paramref name="factoryMethod"/> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<IBuildSession, T1, T2, T3, T4, T5, T6, T7, object> factoryMethod) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), _weight);
    
  }

  /// <summary>
  /// This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  /// <typeparam name="T">The type of parameter</typeparam>
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class ParameterValueTuner<T> : ParameterValueTuner
  {
    public ParameterValueTuner([NotNull] IUnitMatcher parameterMatcher, int weight) : base(parameterMatcher, weight) { }
  }
}