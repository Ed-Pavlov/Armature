using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;

namespace Armature
{
  public class ArgumentStaticTuner // : UnitMatcherExtensibility
  {
    protected readonly Func<IPatternTreeNode, IPatternTreeNode> TuneTreeNodePattern;

    [DebuggerStepThrough]
    public ArgumentStaticTuner(Func<IPatternTreeNode, IPatternTreeNode> tuneTreeNodePattern)
      => TuneTreeNodePattern = tuneTreeNodePattern ?? throw new ArgumentNullException(nameof(tuneTreeNodePattern));

    /// <summary>
    ///   Use the <paramref name="value" /> as an argument for the parameter.
    /// </summary>
    public IArgumentTuner UseValue(object? value)
      => new ArgumentTuner(node => TuneTreeNodePattern(node).UseBuildAction(new Value<object>(value), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="key" />
    /// </summary>
    public IArgumentTuner UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(new BuildArgumentByParameterType(key), BuildStage.Create));
    }

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as key
    /// </summary>
    public IArgumentTuner UseInjectPointIdAsKey()
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(Static<BuildArgumentForMethodWithPointIdAsKey>.Instance, BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod(Func<object> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethod<object>(_ => factoryMethod()), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T>(Func<T?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<T, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node)
         .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node)
         .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node)
         .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node)
         .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), BuildStage.Create));

    /// <summary>
    ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
    /// </summary>
    public IArgumentTuner UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new ArgumentTuner(
        node => TuneTreeNodePattern(node)
         .UseBuildAction(new CreateWithFactoryMethod<object>(factoryMethod), BuildStage.Create));
  }
}
