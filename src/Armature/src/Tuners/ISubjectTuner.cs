using System;
using Armature.Core;

namespace Armature;

public interface ISubjectTuner : ITunerBase
{
  /// <summary>
  /// Amend the weight of the current registration.
  /// </summary>
  ISubjectTuner AmendWeight(short delta);

  /// <summary>
  /// Tune up how to treat types building in the context of <paramref name="type"/>. See <see cref="BuildSession.Stack"/> for details.
  /// </summary>
  /// <param name="type">The type in the "context" of which all subsequent tunings are applied.</param>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  ISubjectTuner Building(Type type, object? tag = null);

  /// <summary>
  /// Tune up how to treat types building in the context of <typeparamref name="T"/>. See <see cref="BuildSession.Stack"/> for details.
  /// </summary>
  /// <typeparam name="T">The type in the "context" of which all subsequent tunings are applied.</typeparam>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  ISubjectTuner Building<T>(object? tag = null);

  /// <summary>
  /// Tune up how to treat the type <paramref name="type"/> during building.
  /// </summary>
  /// <param name="type">The type, the building of which is tuned up by subsequent calls to the tuner.</param>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  IBuildingTuner<object?> Treat(Type type, object? tag = null);

  /// <summary>
  /// Tune up how to treat the type <typeparamref name="T"/> during building.
  /// </summary>
  /// <typeparam name="T">The type, the building of which is tuned up by subsequent calls to the tuner.</typeparam>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  IBuildingTuner<T> Treat<T>(object? tag = null);

  /// <summary>
  /// Tune up how to treat the type <paramref name="openGenericType"/> during building.
  /// </summary>
  /// <param name="openGenericType">The type, the building of which is tuned up by subsequent calls to the tuner.</param>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null);

  /// <summary>
  /// Tune up how to treat all the types inherit the type <paramref name="baseType"/> during building.
  /// </summary>
  /// <param name="baseType">The base type of types being tuned up.</param>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null);

  /// <summary>
  /// Tune up how to treat all the types inherit the type <typeparamref name="T"/> during building.
  /// </summary>
  /// <typeparam name="T">The base type of types being tuned up.</typeparam>
  /// <param name="tag">See <see cref="UnitId"/>.<see cref="UnitId.Tag"/> for details.</param>
  IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null);

  /// <summary>
  /// Tune up how to treat all the types building in this context.
  /// </summary>
  IAllTuner TreatAll();
}
