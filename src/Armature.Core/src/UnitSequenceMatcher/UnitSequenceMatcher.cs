using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Common;
using Armature.Core.Logging;


namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Base class implementing the logic of adding build actions
  /// </summary>
  public abstract class UnitSequenceMatcher : IUnitSequenceMatcher
  {
    private Dictionary<object, List<IBuildAction>>? _buildActions;

    protected UnitSequenceMatcher(int weight) => Weight = weight;

    protected int Weight { get; }

    private Dictionary<object, List<IBuildAction>> LazyBuildAction
    {
      [DebuggerStepThrough] get => _buildActions ??= new Dictionary<object, List<IBuildAction>>();
    }

    public abstract ICollection<IUnitSequenceMatcher> Children { get; }

    public abstract MatchedBuildActions? GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight);

    [DebuggerStepThrough]
    public IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction)
    {
      LazyBuildAction
        .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
        .Add(buildAction);
      return this;
    }

    public abstract bool Equals(IUnitSequenceMatcher other);

    [DebuggerStepThrough]
    protected MatchedBuildActions? GetOwnActions(int matchingWeight)
    {
      if (_buildActions is null) return null;

      var result = new MatchedBuildActions();
      foreach (var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>", GetType().GetShortName(), Weight);

    public void PrintToLog()
    {
      ICollection<IUnitSequenceMatcher>? children = null;
      try {
        children = Children;
      }
      catch { // do nothing
      }
      
      if(children is not null)
        foreach (var child in children)
          using (Log.Block(LogLevel.Info, child.ToString()))
            child.PrintToLog();

      if(_buildActions is not null)
        using (Log.Block(LogLevel.Info, "Build actions"))
          foreach (var pair in _buildActions)
            using (Log.Block(LogLevel.Info, "Stage: {0}", pair.Key))
              foreach (var buildAction in pair.Value)
                if(buildAction is ILogable printable)
                  printable.PrintToLog();
                else
                  Log.Info(buildAction.ToString());
    }
  }
}