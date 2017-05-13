using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using Armature.Logging;

namespace Armature.Core
{
  public abstract class BuildStepBase : IBuildStep
  {
    private Dictionary<object, List<IBuildAction>> _buildSteps;

    private Dictionary<object, List<IBuildAction>> BuildSteps
    {
      get { return _buildSteps ?? (_buildSteps = new Dictionary<object, List<IBuildAction>>()); }
    }

    public BuildStepBase AddBuildAction(object buildStage, IBuildAction weightedBuildStep)
    {
      BuildSteps
        .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
        .Add(weightedBuildStep);
      return this;
    }

    public abstract MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence);

    protected MatchedBuildActions GetOwnActions(int inputWeight)
    {
      var result = new MatchedBuildActions();
      foreach (var pair in BuildSteps)
      {
        var buildStage = pair.Value;

        var actions = buildStage
          .Select(action =>
          {
            Log.Verbose("{0} matches", action);
            return action.WithWeight(inputWeight);
          })
          .ToList();

        if (actions.Count > 0)
        {
          result.Add(pair.Key, actions);
        }
      }
      return result;
    }

    public virtual IBuildStep GetChildBuldStep(ArrayTail<IBuildStep> buildStepsSequence)
    {
      if (buildStepsSequence.Length == 1 && Equals(buildStepsSequence.GetLastItem()))
        return this;
      return null;
    }

    public abstract bool Equals(IBuildStep other);
  }
}