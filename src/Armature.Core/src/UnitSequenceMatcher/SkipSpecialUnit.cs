using System;
using System.Collections;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core.UnitSequenceMatcher
{
  public class SkipSpecialUnit : UnitSequenceMatcherWithChildren, IUnitSequenceMatcher, IEnumerable
  {
    public SkipSpecialUnit(int weight) : base(weight) { }

    public override MatchedBuildActions? GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      var i = 0;
      for(; i < buildingUnitsSequence.Length - 1; i++) // buildingUnitsSequence.Length - 1 always pass last item further
      {
        var unitInfo = buildingUnitsSequence[i];
        if(!unitInfo.Token.IsSpecial())
          break;
      }

      var tail = buildingUnitsSequence.GetTail(i);
      using(Log.AddIndent())
      {
        return GetChildrenActions(Weight + inputWeight, tail);
      }
    }

    IUnitSequenceMatcher IUnitSequenceMatcher.AddBuildAction(object buildStage, IBuildAction buildAction) => throw new NotSupportedException();

    [DebuggerStepThrough]
    public override bool Equals(IUnitSequenceMatcher other) => Equals((object) other);

    [DebuggerStepThrough]
    public override bool Equals(object? obj)
    {
      if(ReferenceEquals(null, obj)) return false;
      if(ReferenceEquals(this, obj)) return true;

      return obj is SkipSpecialUnit other && Weight == other.Weight;
    }

    [DebuggerStepThrough]
    public override int GetHashCode() => Weight.GetHashCode();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
  }
}
