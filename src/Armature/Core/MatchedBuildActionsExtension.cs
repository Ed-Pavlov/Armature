using System;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Common;
using Armature.Logging;
using Armature.Properties;

namespace Armature.Core
{
  public static class MatchedBuildActionsExtension
  {
    /// <summary>
    /// Merges two collections into one
    /// </summary>
    [DebuggerStepThrough]
    public static MatchedBuildActions Merge(this MatchedBuildActions left, MatchedBuildActions right)
    {
      if (left == null) return right;
      if (right == null) return left;

      var result = new MatchedBuildActions();

      foreach (var pair in left)
      {
        List<Weighted<IBuildAction>> resultValue;
        if (!right.TryGetValue(pair.Key, out var rightValue)) // if key is presented only in 'left' dictionary - get value from it
          resultValue = pair.Value;
        else // if key is presented in both dictionaries create a new list and merge items from both
        {
          resultValue = new List<Weighted<IBuildAction>>(pair.Value);
          resultValue.AddRange(rightValue);
        }

        result.Add(pair.Key, resultValue);
      }

      foreach (var pair in right)
        if (!left.ContainsKey(pair.Key))
          result.Add(pair.Key, pair.Value); // for all keys presented in 'right' and not presented in 'left' dictionary get value from 'right'

      return result;
    }

    /// <summary>
    ///   Returns the build action with biggest matching weight for the build stage
    /// </summary>
    /// <exception cref="ArmatureException">Throws if there are more than one action with equal matching weight</exception>
    [CanBeNull]
    public static IBuildAction GetTopmostAction([CanBeNull] this MatchedBuildActions matchedBuildActions, [NotNull] object stage)
    {
      if (stage == null) throw new ArgumentNullException(nameof(stage));

      var actions = matchedBuildActions?.GetValueSafe(stage);
      if (actions == null) return null;

      actions.Sort((l, r) => r.Weight.CompareTo(l.Weight)); // sort descending

      if (actions.Count > 1)
        if (actions[0].Weight == actions[1].Weight)
          throw new ArmatureException("Two or more building actions have the same weight. Enable logging to find the reason");

      return actions[0].Entity;
    }
  }
}