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

    public void Process(UnitBuilder unitBuilder)
    {
      if(unitBuilder.BuildResult != null)
        throw new Exception();

      var result = new List<TFrom>();
      foreach (var targetId in _constructionObjects)
      {
        var buildResult = unitBuilder.Build(targetId);
        if (buildResult != null)
        {
          result.Add((TFrom) buildResult.Value);
          unitBuilder.BuildResult = null;
        }
      }
      unitBuilder.BuildResult = new BuildResult(result);
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {}
  }
}