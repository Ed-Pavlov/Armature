using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

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
