using System;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForProperty"/> tuner.
/// </summary>
[PublicAPI]
public class PropertyArgumentTuner<T> : ArgumentTunerBase<T>
{
  public PropertyArgumentTuner(TuneArgumentRecipient tuneArgumentRecipient) : base(tuneArgumentRecipient) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="PropertyInfo.PropertyType" /> and <paramref name="tag" />
  /// </summary>
  public IArgumentSideTuner UseTag(object tag)
  {
    if(tag is null) throw new ArgumentNullException(nameof(tag));

    return new ArgumentSideTuner(tuner => TuneArgumentRecipientsTo(tuner, Weight).UseBuildAction(new BuildArgumentByPropertyType(tag), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.Tag" /> as a tag
  /// </summary>
  public IArgumentSideTuner UseInjectPointTag()
    => new ArgumentSideTuner(
      node => TuneArgumentRecipientsTo(node, Weight).UseBuildAction(Static.Of<BuildArgumentByPropertyInjectPoint>(), BuildStage.Create));

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public PropertyArgumentTuner<T> AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }
}