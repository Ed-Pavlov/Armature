namespace BeatyBit.Armature;

/// <summary>
/// Tunes (override defaults) target unit's dependencies and how they should be injected.
/// </summary>
public interface IDependencyTuner<out T> : ITunerBase
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  T AmendWeight(int delta);

  /// <summary>
  /// Applies passed rules to the unit. E.g., tuners produced by calling <see cref="ForParameter"/>, <see cref="ForProperty"/>.
  /// </summary>
  /// <param name="sideTuners">See <see cref="ForParameter"/>, <see cref="ForProperty"/>, <see cref="Constructor"/>, and <see cref="Property"/>
  /// for details. Also, custom tuners could be implemented.</param>
  T Using(params ISideTuner[] sideTuners);

  /// <summary>
  /// Object instances or <see cref="IArgumentSideTuner"/> which should be used as arguments for all Units being built during the session
  /// if there are no suitable registrations in the main tree.
  /// </summary>
  /// <param name="arguments">Arguments should be object instances or implementation of <see cref="IArgumentSideTuner"/> interface.</param>
  T UsingArguments(params object[] arguments);

  /// <summary>
  /// Tunes which members of the unit should be used to inject dependencies.
  /// </summary>
  /// <param name="injectionPoints">See <see cref="Constructor"/> and <see cref="Property"/> for details.</param>
  T UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints);
}
