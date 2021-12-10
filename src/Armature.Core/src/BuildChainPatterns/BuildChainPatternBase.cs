using System.Linq;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Base class implementing the logic of adding build actions
/// </summary>
public abstract class BuildChainPatternBase : BuildChainPatternWithChildrenBase
{
  private BuildActionBag? _buildActions;
  private BuildActionBag  LazyBuildAction => _buildActions ??= new BuildActionBag();

  protected BuildChainPatternBase(int weight) : base(weight) { }

  public override BuildActionBag BuildActions => LazyBuildAction;

  [PublicAPI]
  protected WeightedBuildActionBag? GetOwnBuildActions(int inputWeight)
  {
    if(_buildActions is null) return null;

    var matchingWeight = inputWeight + Weight;
    var result = new WeightedBuildActionBag();

    foreach(var pair in _buildActions)
      result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

    return result;
  }

  protected WeightedBuildActionBag? GetOwnOrChildrenBuildActions(BuildChain buildChain, int inputWeight)
  {
    WeightedBuildActionBag? actionBag = null;

    if(buildChain.Length == 1)
    {
      actionBag = GetOwnBuildActions(inputWeight);
      actionBag.WriteToLog(LogLevel.Verbose, "Actions: ");
    }
    else
    { // pass the rest of the chain to children and return their actions
      if(RawChildren is null)
        Log.WriteLine(LogLevel.Trace, "Children: null");
      else
        actionBag = GetChildrenActions(buildChain.GetTail(1), inputWeight);
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