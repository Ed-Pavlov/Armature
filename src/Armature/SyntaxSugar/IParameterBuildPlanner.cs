using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public interface IParameterBuildPlanner
  {
    void RegisterParameterResolver([NotNull] BuildStep buildStep);
  }
}