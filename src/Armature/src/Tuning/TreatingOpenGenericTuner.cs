using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;


namespace Armature;

public class TreatingOpenGenericTuner : UnitSequenceExtensibility
{
  [DebuggerStepThrough]
  public TreatingOpenGenericTuner(IPatternTreeNode parentNode) : base(parentNode) { }

  /// <summary>
  ///   Build an object of the specified <paramref name="openGenericType"/> instead. Also use default creation strategy for that type.
  ///   See <see cref="Default.CreationBuildAction"/> for details.
  /// </summary>
  public FinalTuner AsCreated(Type openGenericType, object? key = null) => As(openGenericType, key).CreatedByDefault();

  /// <summary>
  ///   Build an object of the specified <paramref name="openGenericType"/> instead. 
  /// </summary>
  public OpenGenericCreationTuner As(Type openGenericType, object? key = null)
  {
    ParentNode.UseBuildAction(new RedirectOpenGenericType(openGenericType, key), BuildStage.Create);
    return new OpenGenericCreationTuner(ParentNode, openGenericType, key);
  }

  /// <summary>
  ///   Use default creation strategy for a unit. See <see cref="Default.CreationBuildAction"/> for details.
  /// </summary>
  public FinalTuner AsIs()
  {
    ParentNode.UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return new FinalTuner(ParentNode);
  }
}