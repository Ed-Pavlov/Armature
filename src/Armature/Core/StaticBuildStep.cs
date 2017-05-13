using System.Collections.Generic;
using System.Linq;
using Armature.Common;
using Armature.Logging;

namespace Armature.Core
{
  public abstract class StaticBuildStep : BuildStepBase
  {
    private Dictionary<object, List<IBuildAction>> _buildActions;

    private Dictionary<object, List<IBuildAction>> BuildActions
    {
      get { return _buildActions ?? (_buildActions = new Dictionary<object, List<IBuildAction>>()); }
    }

    public StaticBuildStep AddBuildAction(object buildStage, IBuildAction weightedBuildStep)
    {
      BuildActions
        .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
        .Add(weightedBuildStep);
      return this;
    }

    protected MatchedBuildActions GetOwnActions(int inputWeight)
    {
      var result = new MatchedBuildActions();
      foreach (var pair in BuildActions)
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
  }
}