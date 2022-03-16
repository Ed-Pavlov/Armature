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
  protected bool GetOwnBuildActions(int inputWeight, out WeightedBuildActionBag? actionBag)
  {
    actionBag = null;
    if(_buildActions is null) return false;

    var matchingWeight = inputWeight + Weight;
    actionBag = new WeightedBuildActionBag();

    foreach(var pair in _buildActions)
      actionBag.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

    return true;
  }

  protected bool GetOwnOrChildrenBuildActions(BuildChain buildChain, int inputWeight, out WeightedBuildActionBag? actionBag)
  {
    var result = false;
    actionBag = null;

    if(buildChain.Length > 1)
    { // pass the rest of the chain to children and return their actions
      result = GetChildrenActions(buildChain.GetTail(1), inputWeight, out actionBag);
    }
    else
    { // this patterns matches the target unit, return actions for it
      result = GetOwnBuildActions(inputWeight, out actionBag);
      actionBag.WriteToLog(LogLevel.Verbose, "Actions: ");
    }

    return result;
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