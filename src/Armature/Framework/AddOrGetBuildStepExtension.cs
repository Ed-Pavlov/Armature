using System.Diagnostics;
using System.Linq;
using Armature.Core;

namespace Armature.Framework
{
  public static class AddOrGetBuildStepExtension
  {
    /// <summary>
    ///   Adds the <see cref="unitMatcher" /> into <see cref="parent" />, if <see cref="parent" /> already contains a build step equal to
    ///   <see cref="unitMatcher" /> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    ///   So call it first and then fill a build step with build actions or perform other needed actions.
    /// </summary>
    /// <returns>Newly the build step which will be contained in the <see cref="parent" /> as a child build step</returns>
    [DebuggerStepThrough]
    public static T AddOrGetUnitMatcher<T>(this IUnitSequenceMatcher parent, T unitMatcher)
      where T : IUnitSequenceMatcher
    {
      var existentMatcher = parent.Children.FirstOrDefault(_ => _.Equals(unitMatcher));
      if (existentMatcher != null)
        return (T)existentMatcher;

      parent.Children.Add(unitMatcher);
      return unitMatcher;
    }

    /// <summary>
    ///   Adds the <see cref="unitMatcher" /> into <see cref="parent" />, if <see cref="parent" /> already contains a build step equal to
    ///   <see cref="unitMatcher" /> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    ///   So call it first and then fill a build step with build actions or perform other needed actions
    /// </summary>
    /// <returns>Newly the build step which will be contained in the <see cref="parent" /> as a child build step</returns>
    [DebuggerStepThrough]
    public static T AddOrGetUnitMatcher<T>(this BuildPlansCollection parent, T unitMatcher)
      where T : IUnitSequenceMatcher
    {
      var existentBuildStep = parent.Children.FirstOrDefault(_ => _.Equals(unitMatcher));
      if (existentBuildStep != null)
        return (T)existentBuildStep;

      parent.Children.Add(unitMatcher);
      return unitMatcher;
    }
  }
}