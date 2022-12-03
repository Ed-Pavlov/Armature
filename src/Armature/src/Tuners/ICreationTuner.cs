namespace Armature;

public interface ICreationTuner : ITunerBase
{
  /// <summary>
  /// Amend the weight of the current registration
  /// </summary>
  ICreationTuner AmendWeight(short delta);
  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  ISettingTuner CreatedByDefault();
  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  ISettingTuner CreatedByReflection();
}
