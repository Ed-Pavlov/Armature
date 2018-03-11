using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  ///   Build action redirects building of unit of one type to the unit of another type. E.g. redirecting interface to the implementation
  /// </summary>
  public class RedirectTypeBuildAction : IBuildAction
  {
    private readonly Type _redirectTo;
    private readonly object _token;

    [DebuggerStepThrough]
    public RedirectTypeBuildAction([NotNull] Type redirectTo, [CanBeNull] object token)
    {
      if (redirectTo == null) throw new ArgumentNullException(nameof(redirectTo));
      if (redirectTo.IsGenericTypeDefinition)
        throw new ArgumentException("Type should not be open generic, use RedirectOpenGenericTypeBuildAction for open generics", nameof(redirectTo));

      _redirectTo = redirectTo;
      _token = token;
    }

    public void Process(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult == null)
      {
        var unitInfo = new UnitInfo(_redirectTo, _token);
        Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
        unitBuilder.BuildResult = unitBuilder.Build(unitInfo);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(UnitBuilder unitBuilder) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}: {1}", GetType().Name, _redirectTo);
  }
}