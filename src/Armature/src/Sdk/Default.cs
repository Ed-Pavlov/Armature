using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature.Sdk;

[PublicAPI]
public class Default
{
  /// <summary>
  /// This is the default build action used by <see cref="BuildingTuner{T}.As" />.
  /// You can set your own build action which will be used by these tuners.
  /// </summary>
  public static Func<UnitId, IBuildAction> CreateAsBuildAction { get; protected set; } = id => new Redirect(id);

  /// <summary>
  /// This is the default build action used by <see cref="BuildingTuner{T}.AsCreated{TRedirect}" />.
  /// You can set your own build action which will be used by these tuners.
  /// </summary>
  public static IBuildAction CreationBuildAction { get; protected set; } = Static.Of<CreateByReflection>();

  /// <summary>
  /// This is the default build action used by <see cref="BuildingTuner{T}.AsSingleton" />.
  /// You can set your own build action which will be used by these tuners.
  /// </summary>
  public static Func<IBuildAction> CreateSingletonBuildAction { get; protected set; } = () => new Singleton();
}