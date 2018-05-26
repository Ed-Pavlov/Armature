using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatingOpenGenericTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public TreatingOpenGenericTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="BuildPlansCollectionExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> created by default creation strategy is created and injected.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    /// </summary>
    public Tuner AsCreated(Type openGenericType, object token = null) => As(openGenericType, token).CreatedByDefault();

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="BuildPlansCollectionExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> injected. Tune how it is created by subsequence tuner calls. 
    /// </summary>
    public OpenGenericCreationTuner As(Type openGenericType, object token = null)
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectOpenGenericTypeBuildAction(openGenericType, token));
      return new OpenGenericCreationTuner(UnitSequenceMatcher, openGenericType, token);
    }

    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(UnitSequenceMatcher);
    }
  }
}