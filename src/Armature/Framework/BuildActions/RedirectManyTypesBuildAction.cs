using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class RedirectManyTypesBuildAction<TFrom> : IBuildAction
  {
    private readonly UnitInfo[] _constructionObjects;

    [DebuggerStepThrough]
    public RedirectManyTypesBuildAction(params UnitInfo[] constructionObjects)
    {
      if (constructionObjects.Length == 0) throw new Exception();

      _constructionObjects = constructionObjects;
    }

    public void Process(IBuildSession buildSession)
    {
      if (buildSession.BuildResult != null)
        throw new Exception();

      var result = new List<TFrom>();
      foreach (var targetId in _constructionObjects)
      {
        var buildResult = buildSession.BuildUnit(targetId);
        if (buildResult != null)
        {
          result.Add((TFrom)buildResult.Value);
          buildSession.BuildResult = null;
        }
      }

      buildSession.BuildResult = new BuildResult(result);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }
    
    public override string ToString() => 
      string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _constructionObjects.ToString()));
  }
}