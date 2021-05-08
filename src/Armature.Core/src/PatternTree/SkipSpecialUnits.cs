using System;
using Armature.Core.Logging;

namespace Armature.Core
{
  public class SkipSpecialUnits : PatternTreeNodeWithChildren
  {
    public SkipSpecialUnits(int weight) : base(weight) { }
    public SkipSpecialUnits() : base(0) { }

    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var i = 0;
      for(; i < unitSequence.Length - 1; i++)
      {
        var unitId = unitSequence[i];

        if(unitId.Key is not SpecialKey)
          break;
      }

      WeightedBuildActionBag? actionBag = null;
      using(Log.Deferred(LogLevel.Trace, LogMatchingState))
      {
        actionBag = GetChildrenActions(unitSequence.GetTail(i), inputWeight);
        return actionBag;
      }

      void LogMatchingState(Action? blockContent)
      {
        if(actionBag is not null)
          using(Log.Block(LogLevel.Trace, ToString))
            blockContent?.Invoke();
      }
    }
  }
}
