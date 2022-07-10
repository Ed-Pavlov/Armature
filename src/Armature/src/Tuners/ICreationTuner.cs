namespace Armature;

public interface ICreationTuner
{
  /// <summary>
  /// Amend the weight of the current registration
  /// </summary>
  ICreationTuner AmendWeight(short delta);
  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  IFinalAndContextTuner CreatedByDefault();
  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  IFinalAndContextTuner CreatedByReflection();
}
