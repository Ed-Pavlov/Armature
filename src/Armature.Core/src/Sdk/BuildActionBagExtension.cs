using System;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Internal;

namespace Armature.Core.Sdk;

/// <summary>
/// These methods are implemented as extensions instead of class members in order to be able to operation with 'null' BuildActionBag
/// </summary>
public static class BuildActionBagExtension
{
  /// <summary>
  ///   Merges two collections into one
  /// </summary>
  [DebuggerStepThrough]
  public static WeightedBuildActionBag? Merge(this WeightedBuildActionBag? left, WeightedBuildActionBag? right)
  {
    if(left is null) return right;
    if(right is null) return left;

    var result = new WeightedBuildActionBag();

    foreach(var pair in left)
    {
      List<Weighted<IBuildAction>> resultValue;

      if(!right.TryGetValue(pair.Key, out var rightValue)) // if key is presented only in 'left' dictionary - get value from it
      {
        resultValue = pair.Value;
      }
      else // if key is presented in both dictionaries create a new list and merge items from both
      {
        resultValue = new List<Weighted<IBuildAction>>(pair.Value);
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
  ///   Returns the build action with biggest matching weight for the build stage
  /// </summary>
  /// <exception cref="ArmatureException">Throws if there are more than one action with equal matching weight</exception>
  public static IBuildAction? GetTopmostAction(this WeightedBuildActionBag? buildActionBag, object stage)
  {
    if(stage is null) throw new ArgumentNullException(nameof(stage));

    var actions = buildActionBag?.GetValueSafe(stage);
    if(actions is null) return null;

    actions.Sort((l, r) => r.Weight.CompareTo(l.Weight)); // sort descending

    if(actions.Count > 1)
      if(actions[0].Weight == actions[1].Weight)
      {
        throw new ArmatureException($"Two or more building actions matched with the same weight = {actions[0].Weight:n0}")
             .AddData("Action #1", actions[0])
             .AddData("Action #2", actions[1]);
      }

    return actions[0].Entity;
  }
}