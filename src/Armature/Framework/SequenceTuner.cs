using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public class SequenceTuner
  {
    private readonly IUnitSequenceMatcher _unitSequenceMatcher;

    [DebuggerStepThrough]
    public SequenceTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher)
    {
      if (unitSequenceMatcher is null) throw new ArgumentNullException(nameof(unitSequenceMatcher));

      _unitSequenceMatcher = unitSequenceMatcher;
    }

    public SequenceTuner Building<T>(object token = null) => Building(typeof(T), token);

    public SequenceTuner Building([NotNull] Type type, object token = null)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type(type, token));
      return new SequenceTuner(_unitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    public TreatingTuner<T> Treat<T>(object token = null)
    {
      var unitSequenceMatcher = new WildcardUnitSequenceMatcher(Match.Type<T>(token));
      return new TreatingTuner<T>(_unitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }

    public Tuner TreatAll()
    {
      var unitSequenceMatcher = new AnyUnitSequenceMatcher();
      return new Tuner(_unitSequenceMatcher.AddOrGetUnitSequenceMatcher(unitSequenceMatcher));
    }
  }
}