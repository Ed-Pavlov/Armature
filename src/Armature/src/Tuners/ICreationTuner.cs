using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public interface ICreationTuner : ITunerBase, IInternal<IUnitPattern, IBuildStackPattern>
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  ICreationTuner AmendWeight(int delta);

  /// <summary>
  /// Set that the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build the Unit.
  /// </summary>
  ISettingTuner CreatedByDefault();

  /// <summary>
  /// Set that the <see cref="CreateByReflection"/> build action should be used to build the Unit.
  /// </summary>
  ISettingTuner CreatedByReflection();
}
