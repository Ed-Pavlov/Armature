using Armature.Core;

namespace Armature.Extensibility
{
  /// <summary>
  ///   This interface is used to hide <see cref="PatternTreeNode" /> property from the intellisense list (inheritor should implement it explicitly)
  ///   but provides an access to the <see cref="IPatternTreeNode" /> for extensibility. See usages in Tests.Extensibility project.
  /// </summary>
  public interface IUnitSequenceExtensibility
  {
    IPatternTreeNode PatternTreeNode { get; }
  }
}
