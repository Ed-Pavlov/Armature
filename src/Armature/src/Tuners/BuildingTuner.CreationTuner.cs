using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

public partial class BuildingTuner<T>
{
  public ISettingTuner CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  public ISettingTuner CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  public ICreationTuner AmendWeight(int delta) => AmendWeight<ICreationTuner>(delta, this);

  [PublicAPI]
  protected ISettingTuner CreateBy(IBuildAction buildAction)
  {
    BuildStackPatternSubtree().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }
}
