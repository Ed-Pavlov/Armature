using System;

namespace Armature;

public interface ISubjectTuner : ITunerBase
{
  /// <summary>
  /// Amend the weight of the current registration
  /// </summary>
  ISubjectTuner AmendWeight(short delta);
  /// <summary>
  /// Add build actions for units building in the context of unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  ISubjectTuner Building(Type type, object? tag = null);
  /// <summary>
  /// Add build actions for units building in the context of unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  ISubjectTuner Building<T>(object? tag = null);
  /// <summary>
  /// Add build actions to build a unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  IBuildingTuner<object?> Treat(Type type, object? tag = null);
  /// <summary>
  /// Add build actions to build a unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  IBuildingTuner<T> Treat<T>(object? tag = null);
  /// <summary>
  /// Add build actions applied all generic types match the generic type definition specified by <paramref name="openGenericType"/> in subsequence calls.
  /// </summary>
  IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null);
  /// <summary>
  /// Add build actions applied to all inheritors of <paramref name="baseType"/> in subsequence calls.
  /// </summary>
  IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null);
  /// <summary>
  /// Add build actions applied to all inheritors of <typeparamref name="T"/> in subsequence calls.
  /// </summary>
  IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null);
  /// <summary>
  /// Add build action applied to any building unit in subsequence calls. It's needed to setup common build actions like which constructor to call or
  /// inject dependencies into properties or not.
  /// </summary>
  IAllTuner TreatAll();
}
