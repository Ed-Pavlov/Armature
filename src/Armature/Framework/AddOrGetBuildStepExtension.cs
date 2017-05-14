using System.Linq;
using Armature.Core;

namespace Armature.Framework
{
  public static class AddOrGetBuildStepExtension
  {
    public static T AddOrGetBuildStep<T>(this IBuildStep parent, T child) where T : IBuildStep
    {
      var existentBuildStep = parent.Children.FirstOrDefault(_ => _.Equals(child));
      if (existentBuildStep != null)
        return (T)existentBuildStep;

      parent.AddBuildStep(child);
      return child;
    }

    public static T AddOrGetBuildStep<T>(this BuildPlansCollection parent, T child) where T : IBuildStep
    {
      var existentBuildStep = parent.Children.FirstOrDefault(_ => _.Equals(child));
      if (existentBuildStep != null)
        return (T)existentBuildStep;

      parent.AddBuildStep(child);
      return child;
    }
  }
}