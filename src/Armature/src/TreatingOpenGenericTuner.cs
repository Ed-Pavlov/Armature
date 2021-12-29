using System;
using System.Diagnostics;
using Armature.Core;

namespace Armature;

public class TreatingOpenGenericTuner : TunerBase
{
  [DebuggerStepThrough]
  public TreatingOpenGenericTuner(IBuildChainPattern parentNode) : base(parentNode) { }

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
    ParentNode.UseBuildAction(new RedirectOpenGenericType(openGenericType, tag), BuildStage.Create);
    return new OpenGenericCreationTuner(ParentNode, openGenericType, tag);
  }

  /// <summary>
  /// Use default creation strategy for a unit. See <see cref="Default.CreationBuildAction"/> for details.
  /// </summary>
  public FinalTuner AsIs()
  {
    ParentNode.UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return new FinalTuner(ParentNode);
  }
}
