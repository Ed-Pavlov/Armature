namespace Armature.Core;

public static class BuildStackPatternExtension
{
  /// <inheritdoc cref="IBuildStackPattern.AddBuildAction"/>
  /// <returns>Returns 'this' in order to use fluent syntax</returns>
  public static IBuildStackPattern UseBuildAction(this IBuildStackPattern node, IBuildAction buildAction, object buildStage)
  {
    node.AddBuildAction(buildAction, buildStage);
    return node;
  }
}