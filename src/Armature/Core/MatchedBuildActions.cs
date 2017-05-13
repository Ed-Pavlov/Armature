using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  public class MatchedBuildActions : Dictionary<object, List<Weighted<IBuildAction>>>
  {}

  public static class MatchedActionsExtension
  {
    public static MatchedBuildActions Merge(this MatchedBuildActions left, MatchedBuildActions right)
    {
      if (left == null) return right;
      if (right == null) return left;

      var result = new MatchedBuildActions();

      foreach (var pair in left)
      {
        List<Weighted<IBuildAction>> resultValue;
        List<Weighted<IBuildAction>> rightValue;
        if (!right.TryGetValue(pair.Key, out rightValue)) // if key is presented only in 'left' dictionary - get value from it
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

    [CanBeNull]
    public static IBuildAction GetTopmostAction([CanBeNull] this MatchedBuildActions self, [NotNull] object stage)
    {
      if (self == null) return null;
      if (stage == null) throw new ArgumentNullException("stage");

      var actions = self.GetValueSafe(stage);
      if (actions == null) return null;

      actions.Sort((l, r) => r.Weight.CompareTo(l.Weight)); // sort descending

      for (var i = 1; i < actions.Count; i++)
        if (actions[i - 1].Weight == actions[i].Weight)
          throw new ArmatureException("Two or more building actions have the same weight. Enable logging to find the reason");

      return actions.First().Entity;
    }
  }
}