using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class MethodArgumentTuner : UnitMatcherExtensibility
  {
    [DebuggerStepThrough]
    public MethodArgumentTuner(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

    /// <summary>
    ///   Use the <paramref name="value" /> as an argument for the parameter.
    /// </summary>
    public ITuner UseValue(object? value) => new LastUnitTuner(UnitPattern, new Singleton(value), Weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="key" />
    /// </summary>
    public ITuner UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new LastUnitTuner(UnitPattern, new BuildArgumentForMethodParameter(key), Weight);
    }

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as key
    /// </summary>
    public ITuner UseInjectPointIdAsKey() => new LastUnitTuner(UnitPattern, BuildArgumentForMethodWithPointIdAsKey.Instance, Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod(Func<object> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethod<object>(_ => factoryMethod()), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T>(Func<T?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public ITuner UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new LastUnitTuner(UnitPattern, new CreateWithFactoryMethod<object>(factoryMethod), Weight);
  }
}
