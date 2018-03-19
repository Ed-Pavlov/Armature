using Armature.Core;
using Armature.Properties;

namespace Armature
{
  public interface IBuildPlan
  {
    void Register([NotNull] IUnitSequenceMatcher unitSequenceMatcher);
  }
}