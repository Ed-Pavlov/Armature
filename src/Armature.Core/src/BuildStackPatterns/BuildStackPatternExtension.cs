using System.Diagnostics.CodeAnalysis;

namespace BeatyBit.Armature.Core;

public static class BuildStackPatternExtension
{
  /// <inheritdoc cref="IBuildStackPattern.AddBuildAction"/>
  /// <returns>Returns 'this' in order to use fluent syntax</returns>
  public static IBuildStackPattern UseBuildAction(this IBuildStackPattern node, IBuildAction buildAction, object buildStage)
  {
    node.AddBuildAction(buildAction, buildStage);
    return node;
  }

  [DoesNotReturn]
  public static void ThrowNodeIsAlreadyAddedException(IBuildStackPattern parentNode, IBuildStackPattern node)
    => throw new ArmatureException($"Node '{node}' is already in the tree.")
            .AddData($"{nameof(parentNode)}", parentNode)
            .AddData($"{nameof(node)}", node);
}
