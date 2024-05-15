namespace BeatyBit.Armature;

/// <inheritdoc cref="IDependencyTuner{T}"/>
/// <inheritdoc cref="IContextTuner"/>
public interface ISettingTuner : IDependencyTuner<ISettingTuner>, IContextTuner
{
  /// <summary>
  /// Only one instance of Unit should be build and used for all subsequent requests.
  /// </summary>
  IContextTuner AsSingleton();
}
