using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

public partial class BuildingTuner<T>
{
  ISettingTuner ICreationTuner.CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  ISettingTuner ICreationTuner.CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  ICreationTuner ICreationTuner.AmendWeight(int delta) => AmendWeight<ICreationTuner>(delta, this);

  [PublicAPI]
  protected ISettingTuner CreateBy(IBuildAction buildAction)
  {
    BuildStackPatternSubtree().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }

}
