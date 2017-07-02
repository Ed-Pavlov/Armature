using System;
using System.Linq;
using Armature.Common;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class UnitSequenceWeakMatchingBuildStep : StaticBuildStep
  {
    private readonly UnitInfoMatcher _matcher;

    public UnitSequenceWeakMatchingBuildStep([NotNull] UnitInfoMatcher matcher)
    {
      if (matcher == null) throw new ArgumentNullException("matcher");
      _matcher = matcher;
    }

    /// <summary>
    /// This build step skips <see cref="UnitInfo"/> in <paramref name="matchingPattern"/> which are not matched and gets the next one till find
    /// matched one, then pass to the children the rest of <paramref name="matchingPattern"/> 
    /// </summary>
    public override MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern)
    {
      for (var i = 0; i < matchingPattern.Length; i++)
      {
        var unitInfo = matchingPattern[i];
        if (!_matcher.Matches(unitInfo))
          continue;

        var matchedBuildActions = GetActions(matchingPattern.GetTail(i), _matcher.MatchingWeight + inputMatchingWeight);
        LogMatchResult(matchedBuildActions);
        return matchedBuildActions;
      }
      return null;
    }

    private MatchedBuildActions GetActions(ArrayTail<UnitInfo> buildSequence, int weight)
    {
      return buildSequence.Length == 1
        ? GetOwnActions(weight) // currently building Unit is matched, return our own build steps actions
        : GetChildrenActions(weight, buildSequence.GetTail(1)); // pass the rest of the sequence to children and return their actions
    }

    public override bool Equals(IBuildStep obj)
    {
      var other = obj as UnitSequenceWeakMatchingBuildStep;
      return other != null && _matcher.Equals(other._matcher);
    }

    private void LogMatchResult(MatchedBuildActions result)
    {
      var typeName = GetType().Name;
      if (result == null || result.Count == 0)
        Log.Trace("{0}: no actions", typeName);
      else if (result.Count > 1)
      {
        using (Log.Block(typeName, LogLevel.Trace))
          result.LogMatchedBuildActions(LogLevel.Trace);
      }
      else
      {
        var weightedBuildActions = result.First().Value;
        if (weightedBuildActions.Count == 1)
          Log.Trace("{0}: {1}", typeName, weightedBuildActions[0]);
        else
        {
          using (Log.Block(typeName, LogLevel.Trace))
            foreach (var weightedBuildAction in weightedBuildActions)
              Log.Trace(weightedBuildAction.ToString());
        }
      }
    }
  }
}