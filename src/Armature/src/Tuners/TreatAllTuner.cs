using System;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Tuner is used to tune arguments and injection points for all units w/o any context
/// </summary>
public class TreatAllTuner : ITunerInternal, ITreatAllTuner
{
  public TreatAllTuner(IBuildChainPattern treeRoot, int weight)
  {
    TreeRoot = treeRoot ?? throw new ArgumentNullException(nameof(treeRoot));
    Weight   = weight;
  }

  public ITreatAllTuner AmendWeight(short delta)
  {
    Weight += delta;
    return this;
  }

  public ITreatAllTuner UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);
  public ITreatAllTuner InjectInto(params IInjectPointSideTuner[] propertyIds) => DependencyTuner.InjectInto(this, propertyIds);

  public ITunerInternal?    Parent                                  => null;
  public IBuildChainPattern TreeRoot                                { get; }
  public int                Weight                                  { get; private set; }
  public IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node) => node;
}
