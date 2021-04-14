using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Extensibility;


namespace Armature
{
  public class TreatingOpenGenericTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public TreatingOpenGenericTuner(IScannerTree scannerTree) : base(scannerTree) { }

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="BuildPlansCollectionExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> created by default creation strategy is created and injected.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    /// </summary>
    public Tuner AsCreated(Type openGenericType, object? key = null) => As(openGenericType, key).CreatedByDefault();

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="BuildPlansCollectionExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> injected. Tune how it is created by subsequence tuner calls. 
    /// </summary>
    public OpenGenericCreationTuner As(Type openGenericType, object? key = null)
    {
      ScannerTree.AddBuildAction(BuildStage.Create, new RedirectOpenGenericTypeBuildAction(openGenericType, key));

      return new OpenGenericCreationTuner(ScannerTree, openGenericType, key);
    }

    public Tuner AsIs()
    {
      ScannerTree.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(ScannerTree);
    }
  }
}
