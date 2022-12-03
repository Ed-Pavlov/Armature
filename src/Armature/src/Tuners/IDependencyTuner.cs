namespace Armature;

public interface IDependencyTuner<out T> : ITunerBase
{
  /// <summary>
  /// Amend the weight of the current registration
  /// </summary>
  T AmendWeight(short delta);

  /// <summary>
  /// Applies passed rules to the unit
  /// </summary>
  /// <param name="sideTuners">See <see cref="ForParameter"/>, <see cref="ForProperty"/>, <see cref="Constructor"/>, and <see cref="Property"/>
  /// for details. Also custom tuners could be implemented.</param>
  T Using(params ISideTuner[] sideTuners);

  /// <summary>
  /// Tunes how dependencies of the unit should be built
  /// </summary>
  /// <param name="arguments">Arguments should be object instances or implementation of <see cref="IArgumentSideTuner"/></param>
  T UsingArguments(params object[] arguments);

  /// <summary>
  /// Tunes which members should be used as injection points of the unit
  /// </summary>
  /// <param name="injectionPoints">See <see cref="Constructor"/> and <see cref="Property"/> for details</param>
  T UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints);
}