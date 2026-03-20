using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

public interface ICreationTuner : ITunerBase, IInternal<IUnitPattern, IBuildStackPattern>
{
  /// <inheritdoc cref="ISubjectTuner.AmendWeight"/>
  ICreationTuner AmendWeight(int delta);

  /// <summary>
  /// Specifies that the <see cref="Default"/>.<see cref="Default.CreationBuildAction"/> build action should be used to build the unit.
  /// This is typically used for creating instances using the default creation strategy.
  /// </summary>
  /// <returns>An <see cref="ISettingTuner"/> instance to configure additional settings for the unit.</returns>
  ISettingTuner CreatedByDefault();

  /// <summary>
  /// Specifies that the <see cref="CreateByReflection"/> build action should be used to build the unit.
  /// This creates instances by using reflection to select and invoke a constructor.
  /// </summary>
  /// <returns>An <see cref="ISettingTuner"/> instance to configure additional settings for the unit.</returns>
  ISettingTuner CreatedByReflection();
}
