using System;
using System.Diagnostics;
using Armature.Core;

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

  /// <summary>
  /// Build an object of the specified <paramref name="openGenericType"/> instead.
  /// </summary>
  public OpenGenericCreationTuner As(Type openGenericType, object? tag = null)
  {
    TunedNode.UseBuildAction(new RedirectOpenGenericType(openGenericType, tag), BuildStage.Create);

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);
    return new OpenGenericCreationTuner(unitPattern, TreeRoot, TunedNode, ContextFactory!);
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