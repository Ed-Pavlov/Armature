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
  public MethodArgumentTuner(TuneArgumentRecipient tuneArgumentRecipient) : base(tuneArgumentRecipient) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <paramref name="tag" />
  /// </summary>
  public IArgumentSideTuner UseTag(object tag)
  {
    if(tag is null) throw new ArgumentNullException(nameof(tag));

    return new ArgumentSideTuner(tuner => TuneArgumentRecipientsTo(tuner, Weight).UseBuildAction(new BuildArgumentByParameterType(tag), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a tag
  /// </summary>
  public IArgumentSideTuner UseInjectPointIdAsTag()
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(Static.Of<BuildArgumentByParameterInjectPointId>(), BuildStage.Create));

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public MethodArgumentTuner<T> AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }
}
