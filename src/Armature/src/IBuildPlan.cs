using Armature.Core;


namespace Armature
{
  /// <summary>
  ///   Used for simplifying framework syntax, in order to be able create an object which doesn't have an access to the currently being tuned build plan and
  ///   apply the plan later.
  ///   See usages for details.
  /// </summary>
  public interface IBuildPlan
  {
    /// <summary>
    ///   Apply build plan to the currently building plan
    /// </summary>
    /// <param name="unitSequenceMatcher"></param>
    void Apply(IUnitSequenceMatcher unitSequenceMatcher);
  }
}
