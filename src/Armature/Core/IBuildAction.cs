using JetBrains.Annotations;

namespace Armature.Core
{
  public interface IBuildAction
  {
    void Execute([NotNull] UnitBuilder unitBuilder);
    void PostProcess([NotNull] UnitBuilder unitBuilder);
  }
}