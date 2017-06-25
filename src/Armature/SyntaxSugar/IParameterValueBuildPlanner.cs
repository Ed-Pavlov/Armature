using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IParameterValueBuildPlanner
  {
    void AddBuildParameterValueStepTo([NotNull] BuildStepBase buildStep);
  }
}