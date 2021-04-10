using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;


namespace Armature
{
  public class SequenceTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public SequenceTuner(IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" /> with key <paramref name="key" />
    /// </summary>
    public SequenceTuner Building(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, key));

      return new SequenceTuner(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
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
      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, key));

      return new TreatingTuner(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object
    /// </summary>
    public TreatingTuner<T> Treat<T>(object? key = null)
    {
      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(key));

      return new TreatingTuner<T>(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to add some details to build plan of any building unit in context of currently building one
    /// </summary>
    public Tuner TreatAll()
    {
      var unitSequenceMatcher = new AnyUnitSequenceMatcher();

      return new Tuner(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
  }
}
