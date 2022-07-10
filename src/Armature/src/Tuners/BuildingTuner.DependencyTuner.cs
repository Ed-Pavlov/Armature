namespace Armature;

public partial class BuildingTuner<T>
{
  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints)
    => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  IFinalAndContextTuner IDependencyTuner<IFinalAndContextTuner>.AmendWeight(short delta) => AmendWeight<IFinalAndContextTuner>(delta, this);
}
