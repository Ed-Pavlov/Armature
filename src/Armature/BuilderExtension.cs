using System;
using System.Diagnostics;
using Armature.Core;
using Resharper.Annotations;

namespace Armature
{
  public static class BuilderExtension
  {
    /// <summary>
    ///   Use token for building a unit. See <see cref="UnitInfo" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static Tokenizer UsingToken([NotNull] this Builder builder, [NotNull] object token) => new Tokenizer(token, builder);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" />
    /// </summary>
    [DebuggerStepThrough]
    public static T Build<T>([NotNull] this Builder builder) => builder.Build<T>(null, null);

    /// <summary>
    ///   Builds a Unit registered as type <typeparamref name="T" /> using additional <see cref="parameters" /> they can be values or
    ///   implementation of <see cref="IBuildPlan" />. See <see cref="ForParameter" /> for details.
    /// </summary>
    [DebuggerStepThrough]
    public static T Build<T>([NotNull] this Builder builder, params object[] parameters) => builder.Build<T>(null, parameters);

    /// <summary>
    ///   All other Build... methods should delegate to this one. This is the real implementation
    /// </summary>
    [DebuggerStepThrough]
    private static T Build<T>([NotNull] this Builder builder, [CanBeNull] object token, [CanBeNull] params object[] parameters)
    {
      if (builder == null) throw new ArgumentNullException(nameof(builder));

      BuildPlansCollection sessionalBuildPlans = null;
      if (parameters != null && parameters.Length > 0)
      {
        sessionalBuildPlans = new BuildPlansCollection();
        sessionalBuildPlans
          .TreatAll()
          .UsingParameters(parameters);
      }

      return (T)builder.BuildUnit(new UnitInfo(typeof(T), token), sessionalBuildPlans);
    }

    public struct Tokenizer
    {
      private readonly object _token;
      private readonly Builder _builder;

      public Tokenizer([NotNull] object token, [NotNull] Builder builder)
      {
        if (token == null) throw new ArgumentNullException(nameof(token));

        _token = token;
        _builder = builder;
      }

      [DebuggerStepThrough]
      public T Build<T>() => _builder.Build<T>(_token);

      [DebuggerStepThrough]
      public T Build<T>(params object[] parameters) => _builder.Build<T>(_token, parameters);
    }
  }
}