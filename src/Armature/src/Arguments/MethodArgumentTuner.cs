using System;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> tuner.
/// </summary>
public class MethodArgumentTuner<T> : ArgumentTunerBase<T>
{
  public MethodArgumentTuner(Func<IBuildChainPattern, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="key" />
  /// </summary>
  public IArgumentTuner UseKey(object key)
  {
    if(key is null) throw new ArgumentNullException(nameof(key));

    return new ArgumentTuner(node => AddBuildChainPatternsTo(node).UseBuildAction(new BuildArgumentByParameterType(key), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as key
  /// </summary>
  public IArgumentTuner UseInjectPointIdAsKey()
    => new ArgumentTuner(node => AddBuildChainPatternsTo(node).UseBuildAction(Static.Of<BuildArgumentByParameterInjectPointId>(), BuildStage.Create));
}

/// <inheritdoc cref="MethodArgumentTuner{T}"/>
public class MethodArgumentTuner : MethodArgumentTuner<object?>
{
  public MethodArgumentTuner(Func<IBuildChainPattern, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }
}