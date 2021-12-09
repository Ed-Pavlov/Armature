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
  public PropertyArgumentTuner(Func<IBuildChainPattern, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }

  /// <summary>
  /// For building a value for the parameter use <see cref="PropertyInfo.PropertyType" /> and <paramref name="key" />
  /// </summary>
  public IArgumentTuner UseKey(object key)
  {
    if(key is null) throw new ArgumentNullException(nameof(key));

    return new ArgumentTuner(node => AddBuildChainPatternsTo(node).UseBuildAction(new BuildArgumentByPropertyType(key), BuildStage.Create));
  }

  /// <summary>
  /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as key
  /// </summary>
  public IArgumentTuner UseInjectPointIdAsKey()
    => new ArgumentTuner(node => AddBuildChainPatternsTo(node).UseBuildAction(Static.Of<BuildArgumentByPropertyInjectPointId>(), BuildStage.Create));
}

/// <inheritdoc cref="PropertyArgumentTuner{T}"/>
public class PropertyArgumentTuner : PropertyArgumentTuner<object?>
{
  public PropertyArgumentTuner(Func<IBuildChainPattern, IBuildChainPattern> addBuildChainPatterns) : base(addBuildChainPatterns) { }
}