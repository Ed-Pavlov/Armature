using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public static class BuilderExtension
{
  /// <summary>
  /// Use <paramref name="tag"/> to build a unit. See <see cref="UnitId" /> for details.
  /// </summary>
  [DebuggerStepThrough]
  public static WithTag UsingTag(this Builder builder, object tag) => new(builder, tag);

  /// <summary>
  /// Builds a Unit registered as type <typeparamref name="T" />
  /// </summary>
  /// <returns>Returns an instance or null if null is registered as a unit.</returns>
  /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T? Build<T>(this Builder builder) => builder.Build<T>(null, null);

  /// <summary>
  /// Builds a Unit registered as type <typeparamref name="T" /> passing additional <paramref name="runtimeArguments" /> they can be values or
  /// implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> and <see cref="ForProperty"/> for details.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="runtimeArguments">Additional temporary arguments which could be passed into the build session, they are not stored
  /// anywhere and used only for this build session. Normally, registrations take over these arguments because the weight
  /// of runtime arguments is <see cref="WeightOf.BuildContextPattern.SkipAllUnits"/> + 10.</param>
  /// <returns>Returns an instance or null if null is registered as a unit.</returns>
  /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T? Build<T>(this Builder builder, params object[] runtimeArguments) => builder.Build<T>(null, runtimeArguments);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
  /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IReadOnlyList<object?> BuildAll<T>(this Builder builder) => builder.BuildAll<T>(null);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="runtimeArguments">Additional temporary arguments which could be passed into the build session, they are not stored
  /// anywhere and used only for this build session. Normally, registrations take over these arguments because the weight
  /// of runtime arguments is <see cref="WeightOf.BuildContextPattern.SkipAllUnits"/> + 10.</param>
  /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
  /// <exception cref="ArmatureException">Throws if not unit was built by this or any parent containers</exception>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static IReadOnlyList<object?> BuildAll<T>(this Builder builder, params object[]? runtimeArguments) => builder.BuildAll<T>(null, runtimeArguments);

  /// <summary>
  /// All other BuildAll... methods should delegate to this one. This is the real implementation
  /// </summary>
  private static IReadOnlyList<object?> BuildAll<T>(this Builder builder, object? tag, params object[]? arguments)
  {
    var unitId         = new UnitId(typeof(T), tag);
    var auxPatternTree = CreateAuxPatternTree(arguments);

    var unitList = builder.BuildAllUnits(unitId, auxPatternTree);

    return ReferenceEquals(unitList, Empty<Weighted<BuildResult>>.List)
             ? Empty<object?>.List
             : unitList.Select(_ => _.Entity).Select(buildResult => buildResult.Value).ToArray();
  }

  /// <summary>
  /// All other Build... methods should delegate to this one. This is the real implementation
  /// </summary>
  [DebuggerStepThrough]
  private static T? Build<T>(this Builder builder, object? tag, params object[]? arguments)
  {
    if(builder is null) throw new ArgumentNullException(nameof(builder));

    var unitId      = new UnitId(typeof(T), tag);
    var patternTree = CreateAuxPatternTree(arguments);

    var buildResult = builder.BuildUnit(unitId, patternTree);

    return buildResult.HasValue
             ? (T?) buildResult.Value
             : throw new ArmatureException($"Unit {unitId} is not built").AddData($"{nameof(UnitId)}", unitId);
  }

  private static IBuildChainPattern? CreateAuxPatternTree(object[]? arguments)
  {
    if(arguments is not {Length: > 0}) return null;

    // the logic is patternTree.TreatAll().UsingArguments(arguments), but with increased weight of arguments
    var patternTree = new BuildChainPatternTree();
    var treatAll    = new SkipAllUnits(WeightOf.BuildContextPattern.SkipAllUnits + 10);
    patternTree.Children.Add(treatAll);

    new FinalTuner(treatAll).UsingArguments(arguments);
    return patternTree;
  }

  public readonly struct WithTag
  {
    private readonly object  _tag;
    private readonly Builder _builder;

    public WithTag(Builder builder, object tag)
    {
      _tag     = tag     ?? throw new ArgumentNullException(nameof(tag));
      _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Builds a Unit registered as type <typeparamref name="T" /> with an additional tag passed into <see cref="BuilderExtension.UsingTag"/> method.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Build<T>() => _builder.Build<T>(_tag);

    /// <summary>
    /// Builds a Unit registered as type <typeparamref name="T" /> with an additional tag passed into <see cref="BuilderExtension.UsingTag"/> method
    /// passing additional <paramref name="arguments" /> they can be values or
    /// implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Build<T>(params object[] arguments) => _builder.Build<T>(_tag, arguments);


    /// <summary>
    /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight with an additional tag
    /// passed into <see cref="BuilderExtension.UsingTag"/> method.
    /// This can be useful to build all implementers of an interface.
    /// </summary>
    /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IReadOnlyList<object?> BuildAll<T>() => _builder.BuildAll<T>(_tag);


    /// <summary>
    /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight with an additional
    /// tag passed into <see cref="BuilderExtension.UsingTag"/> method passing additional <paramref name="arguments" /> they can be values or
    /// implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> for details.
    /// This can be useful to build all implementers of an interface.
    /// </summary>
    /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IReadOnlyList<object?> BuildAll<T>(params object[] arguments) => _builder.BuildAll<T>(_tag, arguments);
  }
}
