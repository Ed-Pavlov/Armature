using System;
using Armature.Core;
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

    public void Execute(Build.Session buildSession)
    {
      if (buildSession.BuildResult == null)
        buildSession.BuildResult = buildSession.Build(new UnitInfo(_redirectTo, _token));
    }

    public void PostProcess(Build.Session buildSession)
    {}

    public override string ToString()
    {
      return string.Format("[{0}: _redirectTo={1}]", GetType().Name, _redirectTo);
    }
  }
}