namespace BeatyBit.Armature;

public partial class BuildingTuner<T>
{
  ISettingTuner IDependencyTuner<ISettingTuner>.Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  ISettingTuner IDependencyTuner<ISettingTuner>.UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  ISettingTuner IDependencyTuner<ISettingTuner>.UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints)
    => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  ISettingTuner IDependencyTuner<ISettingTuner>.AmendWeight(int delta) => AmendWeight<ISettingTuner>(delta, this);
}