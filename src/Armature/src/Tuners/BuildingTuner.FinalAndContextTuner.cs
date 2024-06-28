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
    return new SubjectTuner(Parent!, CreateNode);

    // Parent.Building<T>(tag)
    IBuildStackPattern CreateNode() => new SkipTillUnit(_unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern);
  }
}