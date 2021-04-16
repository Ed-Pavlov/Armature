using System;
using Armature.Core;


namespace Armature.Extensibility
{
  public abstract class UnitSequenceExtensibility : IUnitSequenceExtensibility
  {
    protected readonly IQuery Query;

    protected UnitSequenceExtensibility(IQuery query)
      => Query = query ?? throw new ArgumentNullException(nameof(query));

    IQuery IUnitSequenceExtensibility.Query => Query;
  }
}
