using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IBuildPlan
  {
    void Register([NotNull] IUnitSequenceMatcher unitSequenceMatcher);
  }
}