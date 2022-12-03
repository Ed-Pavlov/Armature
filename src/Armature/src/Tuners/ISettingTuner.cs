namespace Armature;

/// <summary>
/// Tunes (overrides defaults) the settings (or environment) in which the target unit will be built and "live" runtime.
/// It includes tuning of target unit's dependencies, dependencies of dependencies and their life time.
/// </summary>
public interface ISettingTuner : IDependencyTuner<ISettingTuner>, IContextTuner
{
  IContextTuner AsSingleton();
}
