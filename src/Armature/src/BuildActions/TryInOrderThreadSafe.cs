using System.Collections.Concurrent;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Thread safe implementation of <see cref="TryInOrder"/> build action.
/// </summary>
/// <inheritdoc />
public class TryInOrderThreadSafe : TryInOrderBase
{
  private readonly ConcurrentDictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

  public TryInOrderThreadSafe() { }
  public TryInOrderThreadSafe(params IBuildAction[] buildActions) : base(buildActions) { }

  protected override void StashBuildAction(IBuildSession buildSession, IBuildAction buildAction) => _effectiveBuildActions.TryAdd(buildSession, buildAction);
  protected override bool DumpBuildAction(IBuildSession buildSession, out IBuildAction buildAction)
    => _effectiveBuildActions.TryRemove(buildSession, out buildAction);
}