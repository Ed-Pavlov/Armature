using System;
using Armature.Core;


namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IPatternTreeNode ParentNode;

    protected UnitSequenceExtensibility(IPatternTreeNode treeNode) => ParentNode = treeNode ?? throw new ArgumentNullException(nameof(treeNode));

    IPatternTreeNode IUnitSequenceExtensibility.PatternTreeNode => ParentNode;
  }
}
