using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Extensibility;


namespace Armature
{
  public class SequenceTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public SequenceTuner(IPatternTreeNode parentNode) : base(parentNode) { }

    /// <summary>
    ///   Configure build plans for the unit representing by <paramref name="type"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public SequenceTuner Building(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));

      var patternMatcher = new FindUnitMatches(new Pattern(type, key));
      return new SequenceTuner(ParentNode.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by type <typeparamref name="T"/>
    ///   See <see cref="BuildSession"/> for details.
    /// </summary>
    public SequenceTuner Building<T>(object? key = null) => Building(typeof(T), key);

    /// <summary>
    ///   Configure build plans for Unit of type <paramref name="type"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner Treat(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));

      var patternMatcher = new FindUnitMatches(new Pattern(type, key));
      return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for Unit of type <typeparamref name="T"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner<T> Treat<T>(object? key = null)
    {
      var patternMatcher = new FindUnitMatches(new Pattern(typeof(T), key));
      return new TreatingTuner<T>(ParentNode.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for any unit building in context of the unit.
    ///   See <see cref="BuildSession"/> for details.
    /// </summary>
    public Tuner TreatAll()
    {
      var patternMatcher = new SkipToLastUnit();
      return new Tuner(ParentNode.GetOrAddNode(patternMatcher));
    }
  }
}
