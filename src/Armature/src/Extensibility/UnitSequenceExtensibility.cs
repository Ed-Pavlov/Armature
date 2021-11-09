using System;
using Armature.Core;


namespace Armature.Extensibility;

public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
{
  protected readonly IPatternTreeNode ParentNode;

  protected UnitSequenceExtensibility(IPatternTreeNode parentNode) => ParentNode = parentNode ?? throw new ArgumentNullException(nameof(parentNode));

  IPatternTreeNode IUnitSequenceExtensibility.PatternTreeNode => ParentNode;
}