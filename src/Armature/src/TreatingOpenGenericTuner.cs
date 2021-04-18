using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;


namespace Armature
{
  public class TreatingOpenGenericTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public TreatingOpenGenericTuner(IPatternTreeNode parentNode) : base(parentNode) { }

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="PatternTreeTunerExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> created by default creation strategy is created and injected.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    /// </summary>
    public Tuner AsCreated(Type openGenericType, object? key = null) => As(openGenericType, key).CreatedByDefault();

    /// <summary>
    ///   When generic type belonging to class described by open generic type passed to <see cref="PatternTreeTunerExtension.TreatOpenGeneric"/>
    ///   is requested to inject, object of generic type <paramref name="openGenericType"/> injected. Tune how it is created by subsequence tuner calls. 
    /// </summary>
    public OpenGenericCreationTuner As(Type openGenericType, object? key = null)
    {
      ParentNode.UseBuildAction(BuildStage.Create, new RedirectOpenGenericType(openGenericType, key));

      return new OpenGenericCreationTuner(ParentNode, openGenericType, key);
    }

    public Tuner AsIs()
    {
      ParentNode.UseBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(ParentNode);
    }
  }
}
