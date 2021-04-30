using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class RootTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public RootTuner(IPatternTreeNode parentNode) : base(parentNode) { }

    /// <summary>
    ///   Configure build plans for the unit representing by <paramref name="type"/>.
    /// </summary>
    public RootTuner Building(Type type, object? key = null)
    {
      if(type is null) throw new ArgumentNullException(nameof(type));

      var patternMatcher = new FindUnitMatches(new Pattern(type, key));
      return new RootTuner(ParentNode.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for the unit representing by type <typeparamref name="T"/>
    ///   See <see cref="BuildSession"/> for details.
    /// </summary>
    public RootTuner Building<T>(object? key = null) => Building(typeof(T), key);

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
    ///   Configure build plans for whole class of open generic types.
    ///   How <paramref name="openGenericType" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingOpenGenericTuner TreatOpenGeneric(Type openGenericType, object? key = null)
    {
      var patternMatcher = new FindUnitMatches(new OpenGenericTypePattern(openGenericType, key), WeightOf.FindUnit | WeightOf.OpenGenericPattern);
      return new TreatingOpenGenericTuner(ParentNode.GetOrAddNode(patternMatcher));
    }
    
    /// <summary>
    ///   Configure build plans for all inheritors of <paramref name="baseType"/>.
    ///   How it should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner TreatInheritorsOf(Type baseType, object? key = null)
    {
      var patternMatcher = new FindUnitMatches(new SubtypePattern(baseType, key), WeightOf.FindUnit | WeightOf.SubtypePattern);
      return new TreatingTuner(ParentNode.GetOrAddNode(patternMatcher));
    }

    /// <summary>
    ///   Configure build plans for all inheritors of <typeparamref name="T" />.
    ///   How <typeparamref name="T" /> should be treated is specified by subsequence calls using returned object.
    /// </summary>
    public TreatingTuner<T> TreatInheritorsOf<T>(object? key = null)
    {
      var patternMatcher = new FindUnitMatches(new SubtypePattern(typeof(T), key), WeightOf.FindUnit | WeightOf.SubtypePattern);
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
