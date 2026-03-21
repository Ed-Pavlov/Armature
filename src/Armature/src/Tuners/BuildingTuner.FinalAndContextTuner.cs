using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

public partial class BuildingTuner<T>
{
  public IContextTuner AsSingleton()
  {
    BuildStackPatternSubtree().UseBuildAction(Default.CreateSingletonBuildAction(), BuildStage.Cache);
    return this;
  }

  public ISubjectTuner BuildingIt()
  {
    return new SubjectTuner(_parent!, CreateNode);

    // Parent.Building<T>(tag)
    IBuildStackPattern CreateNode(int weight) => new SkipTillUnit(_unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern);
  }
}