using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;


namespace Armature
{
  public class SequenceTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public SequenceTuner(IPatternTreeNode patternTreeNode) : base(patternTreeNode) { }

    /// <summary>
    ///   Used to make a build plan for a unit only if it is building in a context of building <paramref name="type" /> with key <paramref name="key" />
    /// </summary>
    public SequenceTuner Building(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));

      var query = new FindUnitMatches(new UnitIdPattern(type, key));
      return new SequenceTuner(PatternTreeNode.AddSubQuery(query));
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
      var query = new FindUnitMatches(new UnitIdPattern(type, key));

      return new TreatingTuner(PatternTreeNode.AddSubQuery(query));
    }

    /// <summary>
    ///   Used to make a build plan for <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object
    /// </summary>
    public TreatingTuner<T> Treat<T>(object? key = null)
    {
      var query = new FindUnitMatches(new UnitIdPattern(typeof(T), key));
      return new TreatingTuner<T>(PatternTreeNode.AddSubQuery(query));
    }

    /// <summary>
    ///   Used to add some details to build plan of any building unit in context of currently building one
    /// </summary>
    public Tuner TreatAll()
    {
      var query = new SkipToLastUnit();

      return new Tuner(PatternTreeNode.AddSubQuery(query));
    }
  }
}
