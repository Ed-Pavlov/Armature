using System;
using System.Diagnostics;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

public class DependencyTuner : TunerBase
{
  [PublicAPI]
  protected int Weight;

  [DebuggerStepThrough]
  public DependencyTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns? contextFactory)
    : base(treeRoot, tunedNode, contextFactory)
    => Weight = 0; //TODO: why not AmendWeight

  /// <summary>
  /// Provides arguments to inject into building unit. See <see cref="ForParameter" /> for details.
  /// </summary>
  public FinalTuner UsingArguments(params object[] arguments)
  {
    if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

    if(arguments.Any(arg => arg is null))
      throw new ArgumentNullException(
        nameof(arguments),
        $"Argument should be either {nameof(IArgumentTuner)} or a not null instance. "
      + $"Use {nameof(ForParameter)} or custom {nameof(IArgumentTuner)} to provide null as an argument for a parameter.");

    foreach(var argument in arguments)
      if(argument is IArgumentTuner argumentTuner)
        argumentTuner.Tune(new TuningContext(TreeRoot, TunedNode, ContextFactory), Weight);
      else if(argument is ITuner)
        throw new ArgumentException($"{nameof(IArgumentTuner)} or instance expected");
      else
      {
        TreeRoot.GetOrAddNode(
                   new IfTargetUnit(
                     new IsAssignableFromType(argument.GetType()),
                     Weight
                   + WeightOf.InjectionPoint.ByTypeAssignability
                   + WeightOf.BuildChainPattern.TargetUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
                .TryAddContext(ContextFactory)
                .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);
      }

    return new FinalTuner(TreeRoot, TunedNode, ContextFactory!);
  }

  public FinalTuner InjectInto(params IInjectPointTuner[] propertyIds)
  {
    if(propertyIds is null) throw new ArgumentNullException(nameof(propertyIds));
    if(propertyIds.Length == 0) throw new ArgumentNullException(nameof(propertyIds), "Specify one or more inject point tuners");

    foreach(var injectPointTuner in propertyIds)
      injectPointTuner.Tune(new TuningContext(TreeRoot, TunedNode, ContextFactory), Weight);

    return new FinalTuner(TreeRoot, TunedNode, ContextFactory!);
  }

  public DependencyTuner AmendWeight(int weight)
  {
    Weight += weight;
    return this;
  }
}