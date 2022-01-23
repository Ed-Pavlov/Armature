using System;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> tuner.
/// </summary>
public class MethodArgumentTuner<T> : ArgumentTunerBase<T, MethodArgumentTuner<T>>
{
  public MethodArgumentTuner(Func<TuningContext, int, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="tag" />
  /// </summary>
  public IArgumentTuner UseTag(object tag)
  {
    if(tag is null) throw new ArgumentNullException(nameof(tag));

    return new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, weight).UseBuildAction(new BuildArgumentByParameterType(tag), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a tag
  /// </summary>
  public IArgumentTuner UseInjectPointIdAsTag()
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, weight)
       .UseBuildAction(Static.Of<BuildArgumentByParameterInjectPointId>(), BuildStage.Create));
}

/// <inheritdoc cref="MethodArgumentTuner{T}"/>
public class MethodArgumentTuner : MethodArgumentTuner<object?>
{
  public MethodArgumentTuner(Func<TuningContext, int, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }
}