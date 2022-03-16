using System.Collections.Concurrent;

namespace Armature.Core;

/// <summary>
/// Thread safe implementation of <see cref="TryInOrder"/> build action.
/// </summary>
public class TryInOrderThreadSafe : TryInOrderBase
{
  private readonly ConcurrentDictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

  public TryInOrderThreadSafe() { }
  public TryInOrderThreadSafe(params IBuildAction[] buildActions) : base(buildActions) { }

  protected override void StoreBuildAction(IBuildSession buildSession, IBuildAction buildAction) => _effectiveBuildActions.TryAdd(buildSession, buildAction);
  protected override bool DiscardBuildAction(IBuildSession buildSession, out IBuildAction buildAction)
    => _effectiveBuildActions.TryRemove(buildSession, out buildAction);
}