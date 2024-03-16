using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

public partial class BuildingTuner<T>
{
  ISettingTuner ICreationTuner.CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  ISettingTuner ICreationTuner.CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  ICreationTuner ICreationTuner.AmendWeight(short delta) => AmendWeight<ICreationTuner>(delta, this);

  [PublicAPI]
  protected ISettingTuner CreateBy(IBuildAction buildAction)
  {
    BuildStackPatternSubtree().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }

}
