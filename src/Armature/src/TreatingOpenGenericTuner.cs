using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public class TreatingOpenGenericTuner : TunerBase
{
  [DebuggerStepThrough]
  public TreatingOpenGenericTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory)
    : base(treeRoot, tunedNode, contextFactory) { }

  /// <summary>
  /// Build an object of the specified <paramref name="openGenericType"/> instead. Also use default creation strategy for that type.
  /// See <see cref="Default.CreationBuildAction"/> for details.
  /// </summary>
  public FinalTuner AsCreated(Type openGenericType, object? tag = null) => As(openGenericType, tag).CreatedByDefault();

  private int Weight = 0; //TODO:

  /// <summary>
  /// Build an object of the specified <paramref name="openGenericType"/> instead.
  /// </summary>
  public OpenGenericCreationTuner As(Type openGenericType, object? tag = null)
  {
    TunedNode.UseBuildAction(new RedirectOpenGenericType(openGenericType, tag), BuildStage.Create);

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.OpenGenericPattern;

    var redirectTargetNode = TreeRoot.GetOrAddNode(new IfTargetUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                     .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new OpenGenericCreationTuner(TreeRoot, redirectTargetNode, AddContextTo);
  }

  /// <summary>
  /// Use default creation strategy for a unit. See <see cref="Default.CreationBuildAction"/> for details.
  /// </summary>
  public FinalTuner AsIs()
  {
    TunedNode.UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return new FinalTuner(TreeRoot, TunedNode, ContextFactory!);
  }
}