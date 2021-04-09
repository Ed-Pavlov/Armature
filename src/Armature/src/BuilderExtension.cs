using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Common;
using Armature.Core.UnitSequenceMatcher;


namespace Armature
{
  public static class BuilderExtension
  {
    /// <summary>
    ///   Use token for building a unit. See <see cref="UnitInfo" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static Tokenizer UsingToken(this Builder builder, object token) => new(token, builder);

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
    private static T Build<T>(this Builder builder, object? token, params object[]? parameters)
    {
      if(builder is null) throw new ArgumentNullException(nameof(builder));

      BuildPlansCollection? sessionalBuildPlans = null;

      if(parameters is {Length: > 0})
      {
        sessionalBuildPlans = new BuildPlansCollection();

        var unitSequenceMatcher = sessionalBuildPlans.AddOrGetUnitSequenceMatcher(new AnyUnitSequenceMatcher());
        Tuner.UsingParameters(unitSequenceMatcher, parameters);
      }

      var unitInfo    = new UnitInfo(typeof(T), token);
      var buildResult = builder.BuildUnit(unitInfo, sessionalBuildPlans);

      return buildResult.HasValue
               ? (T) buildResult.Value!
               : throw new ArmatureException($"Can't build unit <{unitInfo}>").AddData("unitInfo", unitInfo);
    }

    public readonly struct Tokenizer
    {
      private readonly object  _token;
      private readonly Builder _builder;

      public Tokenizer(object token, Builder builder)
      {
        if(builder is null) throw new ArgumentNullException(nameof(builder));
        if(token is null) throw new ArgumentNullException(nameof(token));

        _token   = token;
        _builder = builder;
      }

      /// <summary>
      ///   Calls <see cref="BuilderExtension.Build{T}(Builder, object, object[])" /> with <see cref="_token" /> as token
      /// </summary>
      [DebuggerStepThrough]
      public T Build<T>() => _builder.Build<T>(_token);

      /// <summary>
      ///   Calls <see cref="BuilderExtension.Build{T}(Builder, object, object[])" /> with <see cref="_token" /> as token
      /// </summary>
      [DebuggerStepThrough]
      public T Build<T>(params object[] parameters) => _builder.Build<T>(_token, parameters);
    }
  }
}
