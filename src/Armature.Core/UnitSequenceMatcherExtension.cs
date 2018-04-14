using System.Diagnostics;
using System.Linq;

namespace Armature.Core
{
  public static class UnitSequenceMatcherExtension
  {
    /// <summary>
    ///   Adds the <paramref name="unitSequenceMatcher"/> into <see cref="parent" />, if <see cref="parent" /> already contains the matcher equal to
    ///   <paramref name="unitSequenceMatcher"/> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    /// </summary>
    /// <remarks>Call it first and then fill returned <see cref="IUnitSequenceMatcher"/> with build actions or perform other needed actions.</remarks>
    [DebuggerStepThrough]
    public static T AddOrGetUnitSequenceMatcher<T>(this IUnitSequenceMatcher parent, T unitSequenceMatcher)
      where T : IUnitSequenceMatcher
    {
      var existentMatcher = parent.Children.FirstOrDefault(_ => _.Equals(unitSequenceMatcher));
      if (existentMatcher != null)
        return (T)existentMatcher;

      parent.Children.Add(unitSequenceMatcher);
      return unitSequenceMatcher;
    }
    
    [DebuggerStepThrough]
    public static T AddUniqueUnitMatcher<T>(this IUnitSequenceMatcher parent, T unitSequenceMatcher)
      where T : IUnitSequenceMatcher
    {
      var existentMatcher = parent.Children.FirstOrDefault(_ => _.Equals(unitSequenceMatcher));
      if (existentMatcher != null)
        throw new ArmatureException(string.Format("There is already matcher {0}", unitSequenceMatcher));

      parent.Children.Add(unitSequenceMatcher);
      return unitSequenceMatcher;
    }

    /// <summary>
    ///   Adds the <paramref name="unitSequenceMatcher"/> into <see cref="parent" />, if <see cref="parent" /> already contains the matcher equal to
    ///   <paramref name="unitSequenceMatcher"/> it does not add it but return the existent one. This method is used to add on a build plan or
    ///   make a several registrations regarding one Unit separately.
    /// </summary>
    /// <remarks>Call it first and then fill returned <see cref="IUnitSequenceMatcher"/> with build actions or perform other needed actions.</remarks>
    [DebuggerStepThrough]
    public static T AddOrGetUnitSequenceMatcher<T>(this BuildPlansCollection parent, T unitSequenceMatcher)
      where T : IUnitSequenceMatcher
    {
      var existent = parent.Children.FirstOrDefault(_ => _.Equals(unitSequenceMatcher));
      if (existent != null)
        return (T)existent;

      parent.Children.Add(unitSequenceMatcher);
      return unitSequenceMatcher;
    }
  }
}