using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IParameterMatcherSugar
  {
    IBuildAction BuildAction { get; set; }
    void AddBuildParameterValueStepTo([NotNull] IUnitSequenceMatcher unitSequenceMatcher);
  }
}