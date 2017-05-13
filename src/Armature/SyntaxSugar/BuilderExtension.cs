using System;
using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public static class BuilderExtension
  {
    [DebuggerStepThrough]
    public static Token WithToken([NotNull] this Builder builder, [NotNull] object token)
    {
      return new Token(token, builder);
    }

    [DebuggerStepThrough]
    public static T Build<T>([NotNull] this Builder builder)
    {
      return builder.Build<T>(null, null);
    }

    [DebuggerStepThrough]
    public static T Build<T>([NotNull] this Builder builder, params object[] parameters)
    {
      return builder.Build<T>(null, parameters);
    }

    /// <summary>
    /// All other Build... methods should delegate to this one. This is the real implementation
    /// </summary>
    [DebuggerStepThrough]
    private static T Build<T>([NotNull] this Builder builder, [CanBeNull] object token, [CanBeNull] params object[] parameters)
    {
      if (builder == null) throw new ArgumentNullException("builder");

      IBuildPlansCollection sessionalBuildPlans = null;
      if(parameters != null && parameters.Length > 0)
      {
        sessionalBuildPlans = new BuildPlansCollection();
        sessionalBuildPlans
          .TreatAll()
          .UsingParameters(parameters);
      }

      return (T) builder.BuildUnit(new UnitInfo(typeof(T), token), sessionalBuildPlans);
    }

    public struct Token
    {
      private readonly object _token;
      private readonly Builder _builder;

      public Token([NotNull] object token, [NotNull] Builder builder)
      {
        if (token == null) throw new ArgumentNullException("token");

        _token = token;
        _builder = builder;
      }

      [DebuggerStepThrough]
      public T Build<T>()
      {
        return _builder.Build<T>(_token, null);
      }
      [DebuggerStepThrough]
      public T Build<T>(params object[] parameters)
      {
        return _builder.Build<T>(_token, parameters);
      }
    }
  }
}