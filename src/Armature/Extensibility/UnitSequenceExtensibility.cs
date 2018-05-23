using System;
using Armature.Core;
using Resharper.Annotations;

namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IUnitSequenceMatcher UnitSequenceMatcher;

    protected UnitSequenceExtensibility([NotNull] IUnitSequenceMatcher unitSequenceMatcher) =>
      UnitSequenceMatcher = unitSequenceMatcher ?? throw new ArgumentNullException(nameof(unitSequenceMatcher));

    IUnitSequenceMatcher IUnitSequenceExtensibility.UnitSequenceMatcher => UnitSequenceMatcher;
  }
}