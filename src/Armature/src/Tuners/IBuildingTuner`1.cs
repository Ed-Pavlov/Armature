using System;
using Armature.Core;

namespace Armature;

public interface IBuildingTuner<in T> : IFinalTuner
{
  /// <summary>
  /// Use specified <paramref name="instance"/> as a unit.
  /// </summary>
  void AsInstance(T instance);
  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build.
  /// </summary>
  ICreationTuner As(Type type, object? tag = null);
  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build.
  /// </summary>
  ICreationTuner As<TRedirect>(object? tag = null);
  /// <summary>
  /// Set that the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  IFinalAndContextTuner AsIs();
  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  IFinalAndContextTuner AsCreated(Type type, object? tag = null);
  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  IFinalAndContextTuner AsCreated<TRedirect>(object? tag = null);
  /// <summary>
  /// Use specified <paramref name="factoryMethod"/> to build a unit.
  /// </summary>
  IFinalAndContextTuner AsCreatedWith(Func<T> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod);
  /// <inheritdoc cref="BuildingTuner{T}.AsCreatedWith(System.Func{T})" />
  IFinalAndContextTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod);

  /// <summary>
  /// Amend the weight of the current registration
  /// </summary>
  new IBuildingTuner<T> AmendWeight(short delta);
}
