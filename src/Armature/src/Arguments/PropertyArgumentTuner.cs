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
  public PropertyArgumentTuner(Func<IBuildChainPattern, int, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="PropertyInfo.PropertyType" /> and <paramref name="tag" />
  /// </summary>
  public IArgumentTuner UseTag(object tag)
  {
    if(tag is null) throw new ArgumentNullException(nameof(tag));

    return new ArgumentTuner((node, weight) => AddBuildChainPatternsTo(node, weight).UseBuildAction(new BuildArgumentByPropertyType(tag), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a tag
  /// </summary>
  public IArgumentTuner UseInjectPointIdAsTag()
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight).UseBuildAction(Static.Of<BuildArgumentByPropertyInjectPointId>(), BuildStage.Create));
}

/// <inheritdoc cref="PropertyArgumentTuner{T}"/>
public class PropertyArgumentTuner : PropertyArgumentTuner<object?>
{
  public PropertyArgumentTuner(Func<IBuildChainPattern, int, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }
}
