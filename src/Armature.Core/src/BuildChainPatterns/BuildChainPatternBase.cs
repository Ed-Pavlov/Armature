using System.Linq;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
///   Base class implementing the logic of adding build actions
/// </summary>
public abstract class BuildChainPatternBase : BuildChainPatternWithChildrenBase
{
  private BuildActionBag? _buildActions;
  private BuildActionBag  LazyBuildAction => _buildActions ??= new BuildActionBag();

  protected BuildChainPatternBase(int weight) : base(weight) { }

  public override BuildActionBag BuildActions => LazyBuildAction;

  [PublicAPI]
  protected WeightedBuildActionBag? GetOwnBuildActions(int matchingWeight) //TODO: why dont use Weight property itself
  {
    if(_buildActions is null) return null;

    var result = new WeightedBuildActionBag();

    foreach(var pair in _buildActions)
      result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

    return result;
  }

  protected WeightedBuildActionBag? GetOwnOrChildrenBuildActions(ArrayTail<UnitId> buildChain, int inputWeight)
  {
    WeightedBuildActionBag? actionBag = null;

    if(buildChain.Length > 1)
    { // pass the rest of the chain to children and return their actions
      // using(LogMatchingState())
      {
        if(RawChildren is null)
          Log.WriteLine(LogLevel.Trace, "Children: null");
        else
          actionBag = GetChildrenActions(buildChain.GetTail(1), inputWeight + Weight);
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

  protected override void PrintContentToLog(LogLevel logLevel)
  {
    if(_buildActions is not null)
      using(Log.IndentBlock(logLevel, "BuildActions: ", "[]"))
        foreach(var pair in _buildActions)
        foreach(var buildAction in pair.Value)
          Log.WriteLine(LogLevel.Info, $"{{ Action: {buildAction.ToHoconString()}, Stage: {pair.Key} }}");
  }
}