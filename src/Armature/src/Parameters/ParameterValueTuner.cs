using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class ParameterValueTuner : UnitMatcherExtensibility
  {
    [DebuggerStepThrough]
    public ParameterValueTuner(IUnitPattern unitPattern, int weight)
      : base(unitPattern, weight) { }

    /// <summary>
    ///   Use the <paramref name="value" /> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue(object? value) => new(UnitPattern, new Singleton(value), Weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="key" />
    /// </summary>
    public ParameterValueBuildPlan UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new ParameterValueBuildPlan(UnitPattern, new BuildArgumentForMethodParameter(key), Weight);
    }

    /// <summary>
    ///   For building a value for the parameter use the type of parameter and <see cref="InjectAttribute.InjectionPointId" /> as key
    /// </summary>
    public ParameterValueBuildPlan UseInjectPointIdAsKey() => new(UnitPattern, BuildArgumentForMethodWithPointIdAsKey.Instance, Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod(Func<object> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethod<object>(_ => factoryMethod()), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T>(Func<T?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ParameterValueBuildPlan UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new(UnitPattern, new CreateWithFactoryMethod<object>(factoryMethod), Weight);
  }
}
