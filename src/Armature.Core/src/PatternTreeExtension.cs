using System;
using System.Diagnostics;
using System.Linq;
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
    [DebuggerStepThrough]
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
    [DebuggerStepThrough]
    public static T AddNode<T>(this IPatternTreeNode parentNode, T node) where T : IPatternTreeNode
    {
      if(parentNode is null) throw new ArgumentNullException(nameof(parentNode));

      if(parentNode.Children.Contains(node))
        throw new ArmatureException(string.Format("There is already matcher added '{0}'", node));

      parentNode.Children.Add(node);
      return node;
    }
  }
}
