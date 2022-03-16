using System.Collections;
using System.Collections.Generic;

namespace Armature.Core;

/// <summary>
/// This container is used mostly for "default" build actions applied to any target unit
/// For example by default we want to find attributed constructor and if there is no any get longest constructor, set these two actions in right order
/// into <see cref="TryInOrder" /> to reach such behaviour. If a build action did not build a unit container calls the next one till
/// a unit will be built.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="TryInOrderBase.Add" /> method in order to make possible compact and readable
/// initialization like
/// new TryInOrder
/// {
///  new GetConstructorByInjectPointId(),
///  new GetConstructorWithMaxParametersCount()
/// }
/// </remarks>
public class TryInOrder : TryInOrderBase
{
  private readonly Dictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

  public TryInOrder() { }
  public TryInOrder(params IBuildAction[] buildActions) : base(buildActions) { }

  protected override void StoreBuildAction(IBuildSession buildSession, IBuildAction buildAction) => _effectiveBuildActions.Add(buildSession, buildAction);
  protected override bool DiscardBuildAction(IBuildSession buildSession, out IBuildAction buildAction)
  {
    var result = _effectiveBuildActions.TryGetValue(buildSession, out buildAction);

    if(result)
      _effectiveBuildActions.Remove(buildSession);

    return result;
  }


}