using System;
using Armature.Core;

namespace Armature;

public delegate IBuildChainPattern CreateNode();

public interface IBuildingTuner
{
  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  IBuildingTuner AmendWeight(short delta);
  /// <summary>
  /// Add build actions for units building in the context of unit representing by <paramref name="type"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  IBuildingTuner Building(Type type, object? tag = null);
  /// <summary>
  /// Add build actions for units building in the context of unit representing by <typeparamref name="T"/> and <paramref name="tag"/> in subsequence calls.
  /// </summary>
  IBuildingTuner Building<T>(object? tag = null);
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
  ITreatAllTuner TreatAll();
}

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

  new IBuildingTuner<T> AmendWeight(short delta);
}

public interface ICreationTuner
{
  ICreationTuner AmendWeight(short delta);

  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  IFinalAndContextTuner CreatedByDefault();

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  IFinalAndContextTuner CreatedByReflection();
}

public interface IDependencyTuner<out T>
{
  T AmendWeight(short                         delta);
  T UsingArguments(params object[]            arguments);
  T InjectInto(params     IInjectPointSideTuner[] propertyIds);
}

public interface ITreatAllTuner : IDependencyTuner<ITreatAllTuner> { }

public interface IFinalTuner : IDependencyTuner<IFinalAndContextTuner>
{
  IContextTuner AsSingleton();
}

public interface IContextTuner
{
  IBuildingTuner BuildingIt();
}

public interface IFinalAndContextTuner : IFinalTuner, IContextTuner { }
