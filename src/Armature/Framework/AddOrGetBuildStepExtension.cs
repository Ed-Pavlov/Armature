using System.Linq;
using Armature.Core;

namespace Armature.Framework
{
  public static class AddOrGetBuildStepExtension
  {
    /// <summary>
    /// Adds the <see cref="childBuildStep"/> into <see cref="parent"/>, if <see cref="parent"/> already contains a build step equal to 
    /// <see cref="childBuildStep"/> it does not add it but return the existent one. This method is used to add on a build plan or 
    /// make a several registrations regarding one Unit separately.
    /// So call it first and then fill a build step with build actions or perform other needed actions.
    /// </summary>
    /// <returns>Newly the build step which will be contained in the <see cref="parent"/> as a child build step</returns>
    public static T AddOrGetBuildStep<T>(this IBuildStep parent, T childBuildStep) where T : IBuildStep
    {
      var existentBuildStep = parent.Children.FirstOrDefault(_ => _.Equals(childBuildStep));
      if (existentBuildStep != null)
        return (T)existentBuildStep;

      parent.AddBuildStep(childBuildStep);
      return childBuildStep;
    }

    /// <summary>
    /// Adds the <see cref="childBuildStep"/> into <see cref="parent"/>, if <see cref="parent"/> already contains a build step equal to 
    /// <see cref="childBuildStep"/> it does not add it but return the existent one. This method is used to add on a build plan or 
    /// make a several registrations regarding one Unit separately.
    /// So call it first and then fill a build step with build actions or perform other needed actions
    /// </summary>
    /// <returns>Newly the build step which will be contained in the <see cref="parent"/> as a child build step</returns>
    public static T AddOrGetBuildStep<T>(this BuildPlansCollection parent, T childBuildStep) where T : IBuildStep
    {
      var existentBuildStep = parent.Children.FirstOrDefault(_ => _.Equals(childBuildStep));
      if (existentBuildStep != null)
        return (T)existentBuildStep;

      parent.AddBuildStep(childBuildStep);
      return childBuildStep;
    }
  }
}