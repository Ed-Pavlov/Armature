using System;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForProperty"/> tuner.
/// </summary>
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
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a tag
  /// </summary>
  public IArgumentSideTuner UseInjectPointIdAsTag()
    => new ArgumentSideTuner(
      node => TuneArgumentRecipientsTo(node, Weight).UseBuildAction(Static.Of<BuildArgumentByPropertyInjectPointId>(), BuildStage.Create));

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public PropertyArgumentTuner<T> AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }
}
