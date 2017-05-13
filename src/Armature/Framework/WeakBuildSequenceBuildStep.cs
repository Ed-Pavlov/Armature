using System;
using Armature.Common;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class WeakBuildSequenceBuildStep : BuildStep
  {
    private readonly UnitInfoMatcher _matcher;

    public WeakBuildSequenceBuildStep([NotNull] UnitInfoMatcher matcher)
    {
      if (matcher == null) throw new ArgumentNullException("matcher");
      _matcher = matcher;
    }

    public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
    {
      for (var i = 0; i < buildSequence.Length; i++)
      {
        var unitInfo = buildSequence[i];
        if (!_matcher.Matches(unitInfo))
          continue;

        return GetActions(buildSequence.GetTail(i), _matcher.Weight + inputWeight);
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
      var other = obj as WeakBuildSequenceBuildStep;
      return other != null && _matcher.Euqals(other._matcher);
    }
  }
}