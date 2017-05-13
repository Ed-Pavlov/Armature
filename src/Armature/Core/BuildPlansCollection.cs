using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using Armature.Logging;

namespace Armature.Core
{
  public class BuildPlansCollection : IBuildPlansCollection
  {
    private readonly List<IBuildStep> _children = new List<IBuildStep>();

    public IBuildPlansCollection AddBuildStep(IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      _children.Add(buildStep);
      return this;
    }

    public IBuildStep GetBuildStep(ArrayTail<IBuildStep> buildStepsSequence)
    {
      return _children.SingleOrDefault(child => child.GetChildBuldStep(buildStepsSequence) != null);
    }

    public MatchedBuildActions GetActions(IList<UnitInfo> buildSequence)
    {
      var array = ArrayTail.Of(buildSequence, 0);
      return _children.Aggregate((MatchedBuildActions) null, (current, child) => current.Merge(child.GetBuildActions(0, array)));
    }

     public void PrintLog()
    {
      Log.Info("------ Actions Collection ------");
      foreach (var child in _children)
        Log.Info(child.ToString());
      Log.Info("------/Actions Collection ------");
    }
  }
}