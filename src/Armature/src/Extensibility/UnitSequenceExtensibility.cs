using System;
using Armature.Core;


namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IPatternTreeNode PatternTreeNode;

    protected UnitSequenceExtensibility(IPatternTreeNode patternTreeNode) => PatternTreeNode = patternTreeNode ?? throw new ArgumentNullException(nameof(patternTreeNode));

    IPatternTreeNode IUnitSequenceExtensibility.PatternTreeNode => PatternTreeNode;
  }
}
