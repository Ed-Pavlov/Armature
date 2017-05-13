using JetBrains.Annotations;

namespace Armature.Core
{
  public interface IBuildAction
  {
    void Execute([NotNull] Build.Session buildSession);
    void PostProcess([NotNull] Build.Session buildSession);
  }
}