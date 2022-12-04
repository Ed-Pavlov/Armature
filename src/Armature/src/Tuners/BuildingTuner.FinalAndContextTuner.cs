using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner<T>
{
  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildStackPatternTree"/>. See <see cref="Singleton" /> for details
  /// </summary>
  IContextTuner ISettingTuner.AsSingleton()
  {
    GetContextBranch().UseBuildAction(new Singleton(), BuildStage.Cache);
    return this;
  }

  ISubjectTuner IContextTuner.BuildingIt()
  {
    // Parent.Building<T>(tag)
    IBuildStackPattern CreateNode() => new SkipTillUnit(_unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.SkipTillUnit);

    return new SubjectTuner(Parent!, CreateNode);
  }
}