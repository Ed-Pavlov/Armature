namespace Armature;

public partial class BuildingTuner<T>
{
  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.InjectInto(params IInjectPointTuner[] propertyIds) => DependencyTuner.InjectInto(this, propertyIds);

  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.AmendWeight(short delta) => AmendWeight<IFinalAndContextTuner>(delta, this);
}
