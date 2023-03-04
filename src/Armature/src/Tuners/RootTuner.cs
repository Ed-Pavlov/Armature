using System;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Tuner is used as a not null parent tuner which does not perform any tuning. <see cref="GetOrAddNodeTo"/> returns passed node w/o any changes.
/// </summary>
public class RootTuner : ITuner, IDependencyTuner<RootTuner>
{
  public RootTuner(IBuildStackPattern treeRoot) => TreeRoot = treeRoot ?? throw new ArgumentNullException(nameof(treeRoot));

  public ITuner?            Parent   => null;
  public IBuildStackPattern TreeRoot { get; }
  public int                Weight   => 0;

  public IBuildStackPattern GetOrAddNodeTo(IBuildStackPattern node) => node;

  public RootTuner AmendWeight(short                                      delta)           => throw new NotSupportedException();
  public RootTuner Using(params                ISideTuner[]               sideTuners)      => DependencyTuner.Using(this, sideTuners);
  public RootTuner UsingArguments(params       object[]                   arguments)       => DependencyTuner.UsingArguments(this, arguments);
  public RootTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints) => DependencyTuner.UsingInjectionPoints(this, injectionPoints);
}
