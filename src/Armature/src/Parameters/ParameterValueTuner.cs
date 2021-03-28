using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.BuildActions.Parameter;
using Armature.Extensibility;

namespace Armature
{
  public class ParameterValueTuner : UnitMatcherExtensibility
  {
    [DebuggerStepThrough]
    public ParameterValueTuner(IUnitMatcher unitMatcher, int weight)
      : base(unitMatcher, weight)
    {
    }

    /// <summary>
    ///   Use the <paramref name="value" /> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue(object? value) => new(UnitMatcher, new SingletonBuildAction(value), Weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="token" />
    /// </summary>
    public ParameterValueBuildPlan UseToken(object token)
    {
      if (token is null) throw new ArgumentNullException(nameof(token));

      return new ParameterValueBuildPlan(UnitMatcher, new CreateParameterValueBuildAction(token), Weight);
    }

    /// <summary>
    ///   For building a value for the parameter use the type of parameter and <see cref="InjectAttribute.InjectionPointId" /> as token
    /// </summary>
    public ParameterValueBuildPlan UseInjectPointIdAsToken() => new(
      UnitMatcher,
      CreateParameterValueForInjectPointBuildAction.Instance,
      Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod(Func<object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<object>(_ => factoryMethod()), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T>(Func<T?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2>(Func<T1?, T2?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod(Func<IBuildSession, object> factoryMethod) =>
      new(UnitMatcher, new CreateByFactoryMethodBuildAction<object>(factoryMethod), Weight);
  }
}