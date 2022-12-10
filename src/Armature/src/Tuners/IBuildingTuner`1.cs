using System;
using Armature.Core;

namespace Armature;

public interface IBuildingTuner<in T> : ISettingTuner
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  new IBuildingTuner<T> AmendWeight(short delta);

  /// <summary>
  /// Use the <paramref name="instance"/> as the Unit.
  /// </summary>
  void AsInstance(T instance);

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build.
  /// </summary>
  ICreationTuner As(Type type, object? tag = null);

  /// <summary>
  /// Set that object of the type <typeparamref name="TRedirect"/> should be build.
  /// </summary>
  ICreationTuner As<TRedirect>(object? tag = null);

  /// <summary>
  /// Set that the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build the Unit.
  /// </summary>
  ISettingTuner AsIs();

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build and
  /// the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  ISettingTuner AsCreated(Type type, object? tag = null);

  /// <summary>
  /// Set that object of the type <typeparamref name="TRedirect"/> should be build and
  /// the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  ISettingTuner AsCreated<TRedirect>(object? tag = null);

  /// <summary>
  /// Use specified <paramref name="factoryMethod"/> to build a unit.
  /// </summary>
  ISettingTuner AsCreatedWith(Func<T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1>(Func<T1, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2>(Func<T1, T2, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2, T3>(Func<T1, T2, T3, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  ISettingTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod);
}
