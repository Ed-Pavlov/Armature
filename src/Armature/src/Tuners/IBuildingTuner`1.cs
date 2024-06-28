using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

public interface IBuildingTuner<in T> : ISettingTuner
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  new IBuildingTuner<T> AmendWeight(int delta);

  /// <summary>
  /// Stores passed <paramref name="instance"/> as Unit, that is associates corresponding BuildAction
  /// with the <see cref="BuildStage.Create"/> build stage.
  ///
  /// Important:
  /// If your builder has other build stages which post-processing applies to objects after creation, it's possible that
  /// some of your rules will match the instance and will perform some manipulations with it.
  /// Consider using <see cref="AsSingleton(T)"/> if you want to avoid it.
  /// </summary>
  void AsInstance(T instance);

  /// <summary>
  /// Stores passed <paramref name="instance"/> as cached Unit, that is associates corresponding BuildAction
  /// with the <see cref="BuildStage.Cache"/> build stage.
  /// </summary>
  void AsSingleton(T instance);

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be built.
  /// </summary>
  ICreationTuner As(Type type, object? tag = null);

  /// <summary>
  /// Set that object of the type <typeparamref name="TRedirect"/> should be built.
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
