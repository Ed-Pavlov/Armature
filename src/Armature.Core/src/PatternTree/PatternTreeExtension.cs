using System;
using System.Linq;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  public static class PatternTreeExtension
  {
    /// <summary>
    /// Adds a <paramref name="node" /> to the <see cref="IPatternTreeNode.Children"/> collection of <paramref name="parentNode"/>
    /// if the node does not already exist. Returns the new node, or the existing node if the node already exists.
    /// </summary>
    /// <remarks>Call it first and then fill returned <see cref="IPatternTreeNode" /> with build actions or perform other needed actions due to
    /// it can return other instance of <see cref="IPatternTreeNode"/> then <paramref name="node"/>.</remarks>
    public static T GetOrAddNode<T>(this IPatternTreeNode parentNode, T node) where T : IPatternTreeNode
    {
      if(parentNode is null) throw new ArgumentNullException(nameof(parentNode));
      
      if(parentNode.Children.Contains(node))
        return (T) parentNode.Children.First(_ => _.Equals(node));

      parentNode.Children.Add(node);
      return node;
    }
    
    /// <summary>
    /// Adds the <paramref name="node" /> into <paramref name="parentNode" />.
    /// </summary>
    /// <exception cref="ArmatureException">A node already exists in the collection</exception>
    public static T AddNode<T>(this IPatternTreeNode parentNode, T node) where T : IPatternTreeNode
    {
      if(parentNode is null) throw new ArgumentNullException(nameof(parentNode));

      if(parentNode.Children.Contains(node))
        throw new ArgumentException(string.Format("The same node '{0}' has already been added.", node));

      parentNode.Children.Add(node);
      return node;
    }

    /// <summary>
    ///   Adds a <see cref="IBuildAction" /> for a "to be built" unit which is matched by the branch of the pattern tree represented by this node
    ///   with its parents. 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="buildAction">A build action.</param>
    /// <param name="buildStage">A build stage in which the build action is executed.</param>
    /// <returns>Returns 'this' in order to use fluent syntax</returns>    
    public static IPatternTreeNode UseBuildAction([NotNull] this IPatternTreeNode node, IBuildAction buildAction, object buildStage)
    {
      if(node is null) throw new ArgumentNullException(nameof(node));
      if(buildAction is null) throw new ArgumentNullException(nameof(buildAction));
      if(buildStage is null) throw new ArgumentNullException(nameof(buildStage));

      if(node.BuildActions.TryGetValue(buildStage, out var existedBuildAction))
        throw new InvalidOperationException($"Build action {existedBuildAction} is already registered for the stage {buildStage.ToLogString()}");
      
      node.BuildActions.Add(buildStage, buildAction);
      return node;
    }
  }
}
