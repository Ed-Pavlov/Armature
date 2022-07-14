using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner<T>
{
  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  IFinalAndContextTuner ICreationTuner.CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  IFinalAndContextTuner ICreationTuner.CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  private IFinalAndContextTuner CreateBy(IBuildAction buildAction)
  {
    GetContextBranch().UseBuildAction(buildAction, BuildStage.Create);
    return this;
  }

  ICreationTuner ICreationTuner.AmendWeight(short delta) => AmendWeight<ICreationTuner>(delta, this);
}