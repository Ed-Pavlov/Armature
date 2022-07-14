using Armature.Core;
using Armature.Sdk;

namespace Armature;


public partial class BuildingTuner<T>
{
  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/>. See <see cref="Singleton" /> for details
  /// </summary>
  IContextTuner IFinalTuner.AsSingleton()
  {
    GetContextBranch().UseBuildAction(new Singleton(), BuildStage.Cache);
    return this;
  }

  IBuildingTuner IContextTuner.BuildingIt()
  {
    // (Parent as IRootTuner).Building(_unitPattern:{typeof(T), tag})
    IBuildChainPattern CreateNode() => new SkipTillUnit(_unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.SkipTillUnit);

    return new BuildingTuner(Parent!, CreateNode);
  }
}