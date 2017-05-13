using System;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class RedirectOpenGenericTypeBuildAction : IBuildAction
  {
    private readonly Type _redirectTo;
    private readonly object _token;

    public RedirectOpenGenericTypeBuildAction([NotNull] Type redirectTo, [CanBeNull] object token)
    {
      if (redirectTo == null) throw new ArgumentNullException("redirectTo");
      if (!redirectTo.IsGenericTypeDefinition) throw new ArgumentException("Must be open generic type", "redirectTo");
      _redirectTo = redirectTo;
      _token = token;
    }

    public void Execute(Build.Session buildSession)
    {
      var genericType = _redirectTo.MakeGenericType(buildSession.UnitInfo.GetUnitType().GetGenericArguments());
      buildSession.BuildResult = buildSession.Build(new UnitInfo(genericType, _token));
    }

    public void PostProcess(Build.Session buildSession)
    {}
  }
}