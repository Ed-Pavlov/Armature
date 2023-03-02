using Armature.BuildActions.Creation;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Sdk;

[PublicAPI]
public class Default
{
  /// <summary>
  /// This is the default build action used by <see cref="ICreationTuner.CreatedByDefault" /> and <see cref="BuildingTuner{T}.AsCreated{TRedirect}" />.
  /// You can set your own build action which will be used by these tuners.
  /// </summary>
  public static IBuildAction CreationBuildAction { get; protected set; } = Static.Of<CreateByReflection>();
}
