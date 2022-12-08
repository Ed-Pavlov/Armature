using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public partial class BuildingTuner<T>
{
  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  ISettingTuner ICreationTuner.CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  ISettingTuner ICreationTuner.CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  ICreationTuner ICreationTuner.AmendWeight(short delta) => AmendWeight<ICreationTuner>(delta, this);

  private ISettingTuner CreateBy(IBuildAction buildAction)
  {
    BuildStackPatternSubtree().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }

}