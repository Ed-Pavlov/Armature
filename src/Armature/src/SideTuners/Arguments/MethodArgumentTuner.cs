﻿using System;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> tuner.
/// </summary>
[PublicAPI]
public class MethodArgumentTuner<T> : ArgumentTunerBase<T>
{
  public MethodArgumentTuner(TuneArgumentRecipient tuneArgumentRecipient) : base(tuneArgumentRecipient) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo" />.<see cref="ParameterInfo.ParameterType" /> and <paramref name="tag" />
  /// </summary>
  public IArgumentSideTuner UseTag(object tag)
  {
    if(tag is null) throw new ArgumentNullException(nameof(tag));

    return new ArgumentSideTuner(tuner => TuneArgumentRecipientsTo(tuner, Weight).UseBuildAction(new BuildArgumentByParameterType(tag), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo" />.<see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" /> as a tag
  /// </summary>
  public IArgumentSideTuner UseInjectPointTag()
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(Static.Of<BuildArgumentByParameterInjectPoint>(), BuildStage.Create));

  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  public MethodArgumentTuner<T> AmendWeight(int weight)
  {
    Weight += weight;
    return this;
  }
}
