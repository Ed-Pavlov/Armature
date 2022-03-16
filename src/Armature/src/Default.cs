using System;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

[PublicAPI]
public class Default
{
  /// <summary>
  /// This is the default build action used by <see cref="FinalTuner.AsSingleton" />.
  /// You can set <see cref="ThreadSafeSingleton"/> or your own build action which will be used by this tuner.
  /// </summary>
  public static Func<IBuildAction> CreateSingletonBuildAction { get; protected set; } = () => new Singleton();

  /// <summary>
  /// This is the default build action used by <see cref="CreationTuner.CreatedByDefault" /> and <see cref="TreatingTuner{T}.AsCreated{TRedirect}" />.
  /// You can set your own build action which will be used by these tuners.
  /// </summary>
  public static IBuildAction CreationBuildAction { get; protected set; } = Static.Of<CreateByReflection>();
}