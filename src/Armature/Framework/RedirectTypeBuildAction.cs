using System;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class RedirectTypeBuildAction : IBuildAction
  {
    private readonly Type _redirectTo;
    private readonly object _token;

    public RedirectTypeBuildAction([NotNull] Type redirectTo, [CanBeNull] object token)
    {
      if (redirectTo == null) throw new ArgumentNullException("redirectTo");
      if (redirectTo.IsGenericTypeDefinition) throw new ArgumentException("Type should not be open generic, use RedirectOpenGenericTypeBuildAction for open generics", "redirectTo");
      _redirectTo = redirectTo;
      _token = token;
    }

    public void Execute(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult == null)
      {
        var unitInfo = new UnitInfo(_redirectTo, _token);
        Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
        unitBuilder.BuildResult = unitBuilder.Build(unitInfo);
      }
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {}

    public override string ToString()
    {
      return string.Format("{0}: {1}", GetType().Name, _redirectTo);
    }
  }
}