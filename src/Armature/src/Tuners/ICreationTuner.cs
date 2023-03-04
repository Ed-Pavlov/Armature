using Armature.BuildActions.Creation;
using Armature.Sdk;

namespace Armature;

public interface ICreationTuner : ITunerBase
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  ICreationTuner AmendWeight(short delta);

  /// <summary>
  /// Set that the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build the Unit.
  /// </summary>
  ISettingTuner CreatedByDefault();

  /// <summary>
  /// Set that the <see cref="CreateByReflection"/> build action should be used to build the Unit.
  /// </summary>
  ISettingTuner CreatedByReflection();
}
