using System;
using System.Collections.Generic;
using Armature.Core;

namespace Armature.Framework
{
  public class RedirectManyTypesBuildAction<TFrom> : IBuildAction
  {
    private readonly UnitInfo[] _constructionObjects;

    public RedirectManyTypesBuildAction(params UnitInfo[] constructionObjects)
    {
      _constructionObjects = constructionObjects;
      if(constructionObjects.Length == 0) throw new Exception();
    }

    public void Execute(Build.Session buildSession)
    {
      if(buildSession.BuildResult != null)
        throw new Exception();

      var result = new List<TFrom>();
      foreach (var targetId in _constructionObjects)
      {
        var buildResult = buildSession.Build(targetId);
        if (buildResult != null)
        {
          result.Add((TFrom) buildResult.Value);
          buildSession.BuildResult = null;
        }
      }
      buildSession.BuildResult = new BuildResult(result);
    }

    public void PostProcess(Build.Session buildSession)
    {}
  }
}