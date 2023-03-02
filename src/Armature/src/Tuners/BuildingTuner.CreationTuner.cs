using Armature.BuildActions.Creation;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner<T>
{
  /// <inheritdoc cref="ICreationTuner.CreatedByDefault"/>
  ISettingTuner ICreationTuner.CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  /// <inheritdoc cref="ICreationTuner.CreatedByReflection"/>
  ISettingTuner ICreationTuner.CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  ICreationTuner ICreationTuner.AmendWeight(short delta) => AmendWeight<ICreationTuner>(delta, this);

  private ISettingTuner CreateBy(IBuildAction buildAction)
  {
    BuildStackPatternSubtree().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }

}
