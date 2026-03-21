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
  /// This is the default build action used by <see cref="BuildingTuner{T}.AsSingleton()" />.
  /// You can set your own build action which will be used by these tuners, for example, <see cref="ThreadSafeSingleton"/>.
  /// </summary>
  public static Func<IBuildAction> CreateSingletonBuildAction { get; protected set; } = () => new Singleton();

  /// <summary>
  /// This is the default pattern factory used for matching types in "Treat" operations.
  /// It determines how types are matched when using tuners that treat one type as another.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <b>When to use:</b> Override this property when you need to customize how Armature matches types
  /// during "Treat" operations across your entire build configuration.
  /// </para>
  /// <para>
  /// <b>What it's for:</b> This factory creates patterns that determine whether a requested type
  /// should be matched during dependency resolution. By default, it uses <see cref="UnitPattern"/>
  /// which matches types by exact equality.
  /// </para>
  /// <para>
  /// <b>How to use:</b> Set this property to a custom factory function that takes a <see cref="Type"/>
  /// and an optional tag, and returns an <see cref="IUnitPattern"/> that defines the matching logic.
  /// </para>
  /// <para>
  /// <b>Default behavior:</b> Creates a <see cref="UnitPattern"/>, which matches only the exact
  /// type specified (strict type equality).
  /// </para>
  /// </remarks>
  /// <example>
  /// To use assignability-based matching (interfaces and base classes):
  /// <code>
  /// Default.CreatePatternForTreatType = (type, tag) => new IsAssignableFromType(type, tag);
  /// </code>
  /// </example>
  public static Func<Type, object?, IUnitPattern> CreateUnitPatternInTreatType { get; protected set; } = (type, tag) => new UnitPattern(type, tag);
}