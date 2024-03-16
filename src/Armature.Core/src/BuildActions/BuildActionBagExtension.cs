using System;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// These methods are implemented as extensions instead of class members in order to be able to operation with 'null' BuildActionBag.
/// </summary>
public static class BuildActionBagExtension
{
  /// <summary>
  /// Merges two collections into one
  /// </summary>
  [DebuggerStepThrough]
  public static WeightedBuildActionBag? Merge(this WeightedBuildActionBag? left, WeightedBuildActionBag? right)
  {
    if(left is null) return right;
    if(right is null) return left;

    var result = new WeightedBuildActionBag();

    foreach(var pair in left)
    {
      LeanList<Weighted<IBuildAction>> resultValue;

      if(!right.TryGetValue(pair.Key, out var rightValue)) // if key is presented only in 'left' dictionary - get value from it
        resultValue = pair.Value;
      else // if key is presented in both dictionaries create a new list and merge items from both
      {
        resultValue = new LeanList<Weighted<IBuildAction>>(pair.Value);
        resultValue.AddRange(rightValue);
      }

      result.Add(pair.Key, resultValue);
    }

    foreach(var pair in right)
      if(!left.ContainsKey(pair.Key))
        result.Add(pair.Key, pair.Value); // for all keys presented in 'right' and not presented in 'left' dictionary get value from 'right'

    return result;
  }

  /// <summary>
  /// Returns the build action with biggest matching weight for the build stage.
  /// </summary>
  /// <exception cref="ArmatureException">Throws if there are more than one action with equal matching weight.</exception>
  public static IBuildAction? GetTopmostAction(this WeightedBuildActionBag? buildActionBag, object stage)
  {
    if(stage is null) throw new ArgumentNullException(nameof(stage));

    var actions = buildActionBag?.GetValueSafe(stage);
    if(actions is null) return null;

    var topAction      = actions[0];
    var duplicateIndex = -1;

    for(var i = 1; i < actions.Count; i++)
    {
      var action = actions[i];

      if(action.Weight == topAction.Weight)
        duplicateIndex = i;
      else if(action.Weight > topAction.Weight)
      {
        topAction      = action;
        duplicateIndex = -1;
      }
    }

    if(duplicateIndex > -1)
    {
      throw new ArmatureException("Two or more building actions with the same weight are matched. See log for details.")
           .AddData("Weight", topAction.Weight)
           .AddData("Action #1", topAction)
           .AddData("Action #2", actions[duplicateIndex]);
    }

    return topAction.Entity;
  }
}
