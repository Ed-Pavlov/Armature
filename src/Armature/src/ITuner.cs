using Armature.Core;


namespace Armature
{
  /// <summary>
  /// Inheritors of this interface can be used to create "tuners" which can append the pattern tree at any time, in opposite to static "tuners" like
  /// <see cref="TreatingTuner"/>, <see cref="CreationTuner"/> and so on, which are append pattern tree nodes immediately during the call of their methods.    
  /// </summary>
  /// <remarks>See usages for details</remarks>
  public interface ITuner
  {
    /// <summary>
    /// Append pattern tree nodes to the passed <paramref name="patternTreeNode"/>
    /// </summary>
    void Apply(IPatternTreeNode patternTreeNode);
  }
}
