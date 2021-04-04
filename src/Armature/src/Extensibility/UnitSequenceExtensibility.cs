using System;
using Armature.Core;


namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IUnitSequenceMatcher UnitSequenceMatcher;

    protected UnitSequenceExtensibility(IUnitSequenceMatcher unitSequenceMatcher) =>
      UnitSequenceMatcher = unitSequenceMatcher ?? throw new ArgumentNullException(nameof(unitSequenceMatcher));

    IUnitSequenceMatcher IUnitSequenceExtensibility.UnitSequenceMatcher => UnitSequenceMatcher;
  }
}