using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;

namespace Armature.Core
{
  public abstract class BuildStepBase : IBuildStep
  {
    private HashSet<IBuildStep> _children;

    public abstract MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence);

    public void AddBuildStep(IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      
      if(!LazyChildren.Add(buildStep))
        throw new ArgumentException("Build step is already presented in children collection");
    }

    public bool RemoveBuildStep(IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      
      return _children != null && _children.Remove(buildStep);
    }

    public IEnumerable<IBuildStep> Children
    {
      get { return LazyChildren; }
    }

    public abstract bool Equals(IBuildStep other);

    protected MatchedBuildActions GetChildrenActions(int weight, ArrayTail<UnitInfo> tail)
    {
      return LazyChildren.Aggregate((MatchedBuildActions) null, (current, child) => current.Merge(child.GetBuildActions(weight, tail)));
    }

    private HashSet<IBuildStep> LazyChildren
    {
      get { return _children ?? (_children = new HashSet<IBuildStep>()); }
    }
  }
}