using System.Collections.Generic;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <inheritdoc />
public class TryInOrder : TryInOrderBase
{
  private readonly Dictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

  public TryInOrder() { }
  public TryInOrder(params IBuildAction[] buildActions) : base(buildActions) { }

  protected override void StashBuildAction(IBuildSession buildSession, IBuildAction buildAction) => _effectiveBuildActions.Add(buildSession, buildAction);
  protected override bool DumpBuildAction(IBuildSession buildSession, out IBuildAction buildAction)
  {
    var result = _effectiveBuildActions.TryGetValue(buildSession, out buildAction);
    if(result)
      _effectiveBuildActions.Remove(buildSession);
    return result;
  }
}