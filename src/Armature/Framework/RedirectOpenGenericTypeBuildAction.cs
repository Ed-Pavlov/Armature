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

    public void Execute(UnitBuilder unitBuilder)
    {
      var genericType = _redirectTo.MakeGenericType(unitBuilder.UnitInfo.GetUnitType().GetGenericArguments());
      unitBuilder.BuildResult = unitBuilder.Build(new UnitInfo(genericType, _token));
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {}

    public override string ToString()
    {
      return string.Format("{0}: [{1},{2}]", GetType().Name, _redirectTo, _token ?? "null");
    }
  }
}