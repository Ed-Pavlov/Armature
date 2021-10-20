using System;
using System.Collections.Generic;
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
      WeightedBuildActionBag? actionBag;

      if(unitSequence.Length > 1)
      { // pass the rest of the sequence to children and return their actions
        using(LogMatchingState())
        {
          actionBag = GetChildrenActions(unitSequence.GetTail(1), inputWeight + Weight);
        }
      }
      else
      {
        actionBag = GetOwnBuildActions(inputWeight + Weight);

        if(actionBag is null)
          Log.WriteLine(LogLevel.Trace, () => $"{this}{LogConst.NoMatch}");
        else
          using(Log.Block(LogLevel.Trace, ToString)) // pass group method, do not call ToString
          {
            actionBag.ToLog(LogLevel.Trace);
          }
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
            using(Log.Block(LogLevel.Trace, GetLogLine))
              blockContent();
        });

    public override void PrintToLog()
    {
      ICollection<IPatternTreeNode>? children = null;

      try
      {
        children = Children;
      }
      catch
      { // access to Children could throw an exception, do nothing
      }

      if(children is not null)
        foreach(var child in children)
          using(Log.Block(LogLevel.Info, child.ToString))
          {
            child.PrintToLog();
          }

      if(_buildActions is not null)
        using(Log.Block(LogLevel.Info, "Build actions"))
        {
          foreach(var pair in _buildActions)
          foreach(var buildAction in pair.Value)
            if(buildAction is not ILogable printable)
              Log.WriteLine(LogLevel.Info, $"{buildAction}{{ Stage={pair.Key} }}");
            else
            {
              Log.WriteLine(LogLevel.Info, $"Stage={pair.Key}");
              printable.PrintToLog();
            }
        }
    }
  }
}
