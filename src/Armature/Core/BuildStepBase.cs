using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Base class for a build step implementing a base logic of adding/removing children build steps
  /// and obtainging build actions from them
  /// </summary>
  public abstract class BuildStepBase : IBuildStep
  {
    private HashSet<IBuildStep> _children;

    public abstract MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence);

    public void AddBuildStep([NotNull] IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      
      if(!LazyChildren.Add(buildStep))
        throw new ArgumentException("Build step is already presented in children collection");
    }

    public bool RemoveBuildStep([NotNull] IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      
      return _children != null && _children.Remove(buildStep);
    }

    [NotNull]
    public IEnumerable<IBuildStep> Children
    {
      get { return LazyChildren; }
    }

    public abstract bool Equals(IBuildStep other);

    /// <summary>
    /// Gets matched actions from all children build steps
    /// </summary>
    /// <param name="startWeight">The base weight which can be changed by children build steps</param>
    /// <param name="buildSequence">Build sequence</param>
    protected MatchedBuildActions GetChildrenActions(int startWeight, ArrayTail<UnitInfo> buildSequence)
    {
      return LazyChildren.Aggregate((MatchedBuildActions) null, (current, child) => current.Merge(child.GetBuildActions(startWeight, buildSequence)));
    }

    private HashSet<IBuildStep> LazyChildren
    {
      get { return _children ?? (_children = new HashSet<IBuildStep>()); }
    }
  }
}