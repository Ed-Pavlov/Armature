using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Build action. One or more build actions should be performed to build a unit. Building is two
  /// </summary>
  public interface IBuildAction
  {
    void Execute([NotNull] UnitBuilder unitBuilder);
    void PostProcess([NotNull] UnitBuilder unitBuilder);
  }
}