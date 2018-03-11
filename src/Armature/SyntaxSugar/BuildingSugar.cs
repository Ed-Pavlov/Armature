using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class BuildingSugar
  {
    private readonly BuildPlansCollection _container;
    private readonly IUnitSequenceMatcher _unitSequenceMatcher;

    [DebuggerStepThrough]
    public BuildingSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [NotNull] BuildPlansCollection container)
    {
      if (unitSequenceMatcher is null) throw new ArgumentNullException(nameof(unitSequenceMatcher));
      if (container is null) throw new ArgumentNullException(nameof(container));

      _unitSequenceMatcher = unitSequenceMatcher;
      _container = container;
    }

    public BuildingSugar Building<T>(object token = null) => Building(typeof(T), token);

    public BuildingSugar Building([NotNull] Type type, object token = null)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      var buildStep = new WeakUnitSequenceMatcher(Match.Type(type, token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      return new BuildingSugar(_unitSequenceMatcher.AddOrGetUnitMatcher(buildStep), _container);
    }

    public TreatSugar<T> Treat<T>(object token = null)
    {
      var buildStep = new WeakUnitSequenceMatcher(Match.Type<T>(token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      return new TreatSugar<T>(_unitSequenceMatcher.AddOrGetUnitMatcher(buildStep), _container);
    }

    public AdjusterSugar TreatAll()
    {
      var buildStep = new AnyUnitSequenceMatcher();
      return new AdjusterSugar(_unitSequenceMatcher.AddOrGetUnitMatcher(buildStep), _container);
    }
  }
}