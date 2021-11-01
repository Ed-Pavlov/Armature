using System;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class implementing the logic of adding build actions
  /// </summary>
  public abstract class PatternTreeNodeBase : PatternTreeNodeWithChildrenBase
  {
    private BuildActionBag? _buildActions;
    private BuildActionBag  LazyBuildAction => _buildActions ??= new BuildActionBag();

    protected PatternTreeNodeBase(int weight) : base(weight) { }

    public override BuildActionBag BuildActions => LazyBuildAction;

    protected WeightedBuildActionBag? GetOwnBuildActions(int matchingWeight)
    {
      if(_buildActions is null) return null;

      var result = new WeightedBuildActionBag();

      foreach(var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    protected WeightedBuildActionBag? GetOwnOrChildrenBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      WeightedBuildActionBag? actionBag = null;

      if(unitSequence.Length > 1)
      { // pass the rest of the sequence to children and return their actions
        // using(LogMatchingState())
        {
          if(RawChildren is null)
            Log.WriteLine(LogLevel.Trace, "Children: null");
          else
            actionBag = GetChildrenActions(unitSequence.GetTail(1), inputWeight + Weight);
        }
      }
      else
      {
        actionBag = GetOwnBuildActions(inputWeight + Weight);
        Log.Write(LogLevel.Verbose, "Action: ");
        actionBag.WriteToLog(LogLevel.Verbose);

        // if(actionBag is null)
        //   Log.WriteLine(LogLevel.Trace, () => $"{this}{LogConst.NoMatch}");
        // else
        //   using(Log.NamedBlock(LogLevel.Trace, ToString)) // pass group method, do not call ToString
        //   {
        //     actionBag.WriteToLog(LogLevel.Trace);
        //   }
      }

      return actionBag;
    }

    private IDisposable LogMatchingState()
      => Log.Deferred(
        LogLevel.Trace,
        blockContent =>
        {
          string GetLogLine() => $"{this} => pass further";

          if(blockContent is null)
            Log.WriteLine(LogLevel.Trace, GetLogLine);
          else
            using(Log.NamedBlock(LogLevel.Trace, GetLogLine))
              blockContent();
        });

    protected override void PrintContentToLog(LogLevel logLevel)
    {
      if(_buildActions is not null)
        using(Log.IndentBlock(logLevel, "BuildActions: ", "[]"))
          foreach(var pair in _buildActions)
          foreach(var buildAction in pair.Value)
            Log.WriteLine(LogLevel.Info, $"{{ Action: {buildAction.ToHoconString()}, Stage: {pair.Key} }}");
    }
  }
}