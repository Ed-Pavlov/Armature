using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IParameterValueBuildPlan
  {
    void Register([NotNull] IUnitSequenceMatcher unitSequenceMatcher);
  }
}