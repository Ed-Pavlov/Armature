using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;


namespace Armature
{
  public class SequenceTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public SequenceTuner(IScannerTree scannerTree) : base(scannerTree) { }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" /> with key <paramref name="key" />
    /// </summary>
    public SequenceTuner Building(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));
      
      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(type, key));
      return new SequenceTuner(ScannerTree.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <typeparamref name="T" /> with key <paramref name="key" />
    /// </summary>
    public SequenceTuner Building<T>(object? key = null) => Building(typeof(T), key);

    /// <summary>
    ///   Used to make a build plan for Unit of type <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner Treat(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));
      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(new UnitId(type, key)));

      return new TreatingTuner(ScannerTree.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object
    /// </summary>
    public TreatingTuner<T> Treat<T>(object? key = null)
    {
      var unitSequenceMatcher = new SkipToUnit(new UnitIdMatcher(typeof(T), key));
      return new TreatingTuner<T>(ScannerTree.AddItem(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to add some details to build plan of any building unit in context of currently building one
    /// </summary>
    public Tuner TreatAll()
    {
      var unitSequenceMatcher = new SkipToLastUnit();

      return new Tuner(ScannerTree.AddItem(unitSequenceMatcher));
    }
  }
}
