using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Common;

namespace Armature
{
  public static class BuilderExtension
  {
    /// <summary>
    ///   Use key for building a unit. See <see cref="UnitId" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static Keyer UsingKey(this Builder builder, object key) => new(key, builder);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" />
    /// </summary>
    [DebuggerStepThrough]
    public static T Build<T>(this Builder builder) => builder.Build<T>(null, null);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" /> using additional <paramref name="parameters" /> they can be values or
    ///   implementation of <see cref="IBuildPlan" />. See <see cref="ForParameter" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static T Build<T>(this Builder builder, params object[] parameters) => builder.Build<T>(null, parameters);

    /// <summary>
    ///   All other Build... methods should delegate to this one. This is the real implementation
    /// </summary>
    [DebuggerStepThrough]
    private static T Build<T>(this Builder builder, object? key, params object[]? parameters)
    {
      if(builder is null) throw new ArgumentNullException(nameof(builder));

      BuildPlansCollection? sessionalBuildPlans = null;

      if(parameters is {Length: > 0})
      {
        sessionalBuildPlans = new BuildPlansCollection();

        sessionalBuildPlans
        .TreatAll()
        .UsingParameters(parameters);
      }

      var unitInfo    = new UnitId(typeof(T), key);
      var buildResult = builder.BuildUnit(unitInfo, sessionalBuildPlans);

      return buildResult.HasValue
               ? (T) buildResult.Value!
               : throw new ArmatureException($"Can't build unit <{unitInfo}>").AddData("unitInfo", unitInfo);
    }

    public readonly struct Keyer
    {
      private readonly object  _key;
      private readonly Builder _builder;

      public Keyer(object key, Builder builder)
      {
        if(builder is null) throw new ArgumentNullException(nameof(builder));
        if(key is null) throw new ArgumentNullException(nameof(key));

        _key   = key;
        _builder = builder;
      }

      /// <summary>
      ///   Calls <see cref="BuilderExtension.Build{T}(Builder, object, object[])" /> with <see cref="_key" /> as key
      /// </summary>
      [DebuggerStepThrough]
      public T Build<T>() => _builder.Build<T>(_key);

      /// <summary>
      ///   Calls <see cref="BuilderExtension.Build{T}(Builder, object, object[])" /> with <see cref="_key" /> as key
      /// </summary>
      [DebuggerStepThrough]
      public T Build<T>(params object[] parameters) => _builder.Build<T>(_key, parameters);
    }
  }
}
