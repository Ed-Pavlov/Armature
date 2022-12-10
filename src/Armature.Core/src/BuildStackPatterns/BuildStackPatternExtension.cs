﻿using System;
using System.Collections.Generic;
using Armature.Core.Internal;
using Armature.Core.Sdk;

namespace Armature.Core;

public static class BuildStackPatternExtension
{
  /// <summary>
  /// Adds a <paramref name="node" /> as a child to <paramref name="parentNode"/> if the node is not already added.
  /// Returns the new node, or the existing node if the node already added.
  /// </summary>
  /// <remarks>Call it first and then fill returned <see cref="IBuildStackPattern" /> with build actions or perform other needed actions due to
  /// it can return other instance of <see cref="IBuildStackPattern"/> then passed <paramref name="node"/>.</remarks>
  public static T GetOrAddNode<T>(this IBuildStackPattern parentNode, T node) where T : IBuildStackPattern
  {
    if(parentNode is null) throw new ArgumentNullException(nameof(parentNode));
    if(node is null) throw new ArgumentNullException(nameof(node));

    if(parentNode.Children.TryGetValue(node, out var actualNode))
      return (T) actualNode;

    parentNode.Children.Add(node);
    return node;
  }

  /// <summary>
  /// Adds the <paramref name="node" /> as a child to <paramref name="parentNode" />.
  /// </summary>
  /// <exception cref="ArmatureException">A node is already in the tree.</exception>
  public static T AddNode<T>(this IBuildStackPattern parentNode, T node, string? exceptionMessage = null) where T : IBuildStackPattern
  {
    if(parentNode is null) throw new ArgumentNullException(nameof(parentNode));
    if(node is null) throw new ArgumentNullException(nameof(node));

    if(!parentNode.Children.Add(node))
      throw new ArmatureException(exceptionMessage ?? $"Node '{node}' is already in the tree.")
           .AddData($"{nameof(parentNode)}", parentNode)
           .AddData($"{nameof(node)}", node);

    return node;
  }

  /// <summary>
  /// Adds a <see cref="IBuildAction" /> which will be called to build a Target Unit matched by the branch of the build stack pattern tree represented
  /// by this node with its parents.
  /// </summary>
  /// <param name="node"></param>
  /// <param name="buildAction">A build action.</param>
  /// <param name="buildStage">A build stage in which the build action is executed.</param>
  /// <returns>Returns 'this' in order to use fluent syntax</returns>
  public static IBuildStackPattern UseBuildAction(this IBuildStackPattern node, IBuildAction buildAction, object buildStage)
  {
    if(node is null) throw new ArgumentNullException(nameof(node));
    if(buildAction is null) throw new ArgumentNullException(nameof(buildAction));
    if(buildStage is null) throw new ArgumentNullException(nameof(buildStage));

    var list = node.BuildActions.GetOrCreateValue(buildStage, () => new List<IBuildAction>());

    if(!list.Contains(buildAction))
      list.Add(buildAction);

    return node;
  }
}