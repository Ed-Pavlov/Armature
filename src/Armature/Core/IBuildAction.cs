using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Build action. One or more build actions should be performed to build a unit. Building is two-pass process, each build action
  ///   called twice. <see cref="PostProcess" /> is called in reverse order of <see cref="Process" />
  /// </summary>
  public interface IBuildAction
  {
    /// <summary>
    ///   This method is called first for all matched actions in direct order.
    ///   Once <see cref="IBuildSession.BuildResult" /> is set by any action, no other actions <see cref="Process"/> are called.
    /// </summary>
    void Process([NotNull] IBuildSession buildSession);

    /// <summary>
    ///   This method is called when all actions for which <see cref="Process" /> was called are rewind after the unit is just built
    /// </summary>
    void PostProcess([NotNull] IBuildSession buildSession);
  }
}