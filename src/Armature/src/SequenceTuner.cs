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
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" /> with token <paramref name="token" />
    /// </summary>
    public SequenceTuner Building(Type type, object? token = null)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new SequenceTuner(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <typeparamref name="T" /> with token <paramref name="token" />
    /// </summary>
    public SequenceTuner Building<T>(object? token = null) => Building(typeof(T), token);

    /// <summary>
    ///   Used to make a build plan for Unit of type <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner Treat(Type type, object? token = null)
    {
      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new TreatingTuner(UnitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
    
    /// <summary>
    ///   Used to make a build plan for <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object
    /// </summary>
    public TreatingTuner<T> Treat<T>(object? token = null)
    {
      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
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