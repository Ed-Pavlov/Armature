using Armature.Core;
using Resharper.Annotations;

namespace Armature
{
  public interface IBuildPlan
  {
    void Register([NotNull] IUnitSequenceMatcher unitSequenceMatcher);
  }
}