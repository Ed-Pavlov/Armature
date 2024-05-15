using BeatyBit.Armature.Core;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

public partial class BuildingTuner<T>
{
  IContextTuner ISettingTuner.AsSingleton()
  {
    BuildStackPatternSubtree().UseBuildAction(new Singleton(), BuildStage.Cache);
    return this;
  }

  ISubjectTuner IContextTuner.BuildingIt()
  {
    // Parent.Building<T>(tag)
    IBuildStackPattern CreateNode() => new SkipTillUnit(_unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern);

    return new SubjectTuner(Parent!, CreateNode);
  }
}