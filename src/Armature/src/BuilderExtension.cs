using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core;

namespace Armature
{
  public static class BuilderExtension
  {
    /// <summary>
    ///   Use key for building a unit. See <see cref="UnitId" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static WithKey UsingKey(this Builder builder, object key) => new(builder, key);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" />
    /// </summary>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    [DebuggerStepThrough]
    public static T? Build<T>(this Builder builder) => builder.Build<T>(null, null);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" /> passing additional <paramref name="arguments" /> they can be values or
    ///   implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> for details.
    /// </summary>
    /// <returns>Returns an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if unit wasn't built by this or any parent containers</exception>
    [DebuggerStepThrough]
    public static T? Build<T>(this Builder builder, params object[] arguments) => builder.Build<T>(null, arguments);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
    ///   This can be useful to build all implementers of an interface.
    /// </summary>
    /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
    public static IReadOnlyList<object?> BuildAll<T>(this Builder builder) => builder.BuildAll<T>(null);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
    ///   This can be useful to build all implementers of an interface.
    /// </summary>
    /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
    /// <exception cref="ArmatureException">Throws if not unit was built by this or any parent containers</exception>
    public static IReadOnlyList<object?> BuildAll<T>(this Builder builder, params object[]? arguments) => builder.BuildAll<T>(null, arguments);

    /// <summary>
    ///   All other BuildAll... methods should delegate to this one. This is the real implementation
    /// </summary>
    private static IReadOnlyList<object?> BuildAll<T>(this Builder builder, object? key, params object[]? arguments)
    {
      var unitId     = new UnitId(typeof(T), key);
      var buildPlans = CreateAuxPatternTree(arguments);

      var unitList = builder.BuildAllUnits(unitId, buildPlans);

      return ReferenceEquals(unitList, Empty<Weighted<BuildResult>>.List)
               ? Empty<object?>.List
               : unitList.Select(_ => _.Entity).Select(buildResult => buildResult.Value).ToArray();
    }

    /// <summary>
    ///   All other Build... methods should delegate to this one. This is the real implementation
    /// </summary>
    [DebuggerStepThrough]
    private static T? Build<T>(this Builder builder, object? key, params object[]? arguments)
    {
      if(builder is null) throw new ArgumentNullException(nameof(builder));

      var unitId      = new UnitId(typeof(T), key);
      var patternTree = CreateAuxPatternTree(arguments);

      var buildResult = builder.BuildUnit(unitId, patternTree);

      //TODO: check code for build plan name
      return buildResult.HasValue
               ? (T?)buildResult.Value
               : throw new ArmatureException($"Unit {unitId} is not built").AddData($"{nameof(unitId)}", unitId);
    }

    private static IPatternTreeNode? CreateAuxPatternTree(object[]? arguments)
    {
      if(arguments is not { Length: > 0 }) return null;

      var buildPlans = new PatternTree();

      // the logic is, but with increased weight of arguments
      // buildPlans
      //  .TreatAll()
      //  .UsingArguments(arguments);

      var treatAll = new SkipAllUnits(WeightOf.SkipAllUnits + 10);
      buildPlans.Children.Add(treatAll);

      new FinalTuner(treatAll)
       .UsingArguments(arguments);

      return buildPlans;
    }

    public readonly struct WithKey
    {
      private readonly object  _key;
      private readonly Builder _builder;

      public WithKey(Builder builder, object key)
      {
        _key     = key     ?? throw new ArgumentNullException(nameof(key));
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
      }

      /// <summary>
      ///   Builds a Unit registered as type <typeparamref name="T" /> with an additional key passed into <see cref="BuilderExtension.UsingKey"/> method.
      /// </summary>
      [DebuggerStepThrough]
      public T? Build<T>() => _builder.Build<T>(_key);

      /// <summary>
      ///   Builds a Unit registered as type <typeparamref name="T" /> with an additional key passed into <see cref="BuilderExtension.UsingKey"/> method
      ///   passing additional <paramref name="arguments" /> they can be values or
      ///   implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> for details.
      /// </summary>
      [DebuggerStepThrough]
      public T? Build<T>(params object[] arguments) => _builder.Build<T>(_key, arguments);


      /// <summary>
      ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight with an additional key
      ///   passed into <see cref="BuilderExtension.UsingKey"/> method.
      ///   This can be useful to build all implementers of an interface.
      /// </summary>
      /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
      /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
      [DebuggerStepThrough]
      public IReadOnlyList<object?> BuildAll<T>() => _builder.BuildAll<T>(_key);


      /// <summary>
      ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight with an additional
      ///   key passed into <see cref="BuilderExtension.UsingKey"/> method passing additional <paramref name="arguments" /> they can be values or
      ///   implementation of <see cref="ITuner" />. See <see cref="ForParameter" /> for details.
      ///   This can be useful to build all implementers of an interface.
      /// </summary>
      /// <returns>Returns a list of built units or null if no an instance or null if null is registered as a unit.</returns>
      /// <exception cref="ArmatureException">Throws if no unit was built by this or any parent containers</exception>
      [DebuggerStepThrough]
      public IReadOnlyList<object?> BuildAll<T>(params object[] arguments) => _builder.BuildAll<T>(_key, arguments);
    }
  }
}