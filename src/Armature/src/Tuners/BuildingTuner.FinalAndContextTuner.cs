using Armature.BuildActions.Caching;
using Armature.Core;
using WeightOf = Armature.Sdk.WeightOf;

namespace Armature;

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