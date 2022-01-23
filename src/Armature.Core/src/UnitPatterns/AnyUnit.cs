using System;
using Armature.Core.Sdk;

namespace Armature.Core;

public class AnyUnit : IUnitPattern
{
  private readonly object _tag;

  public AnyUnit() : this(SpecialTag.Any) { }
  public AnyUnit(object tag) => _tag = tag ?? throw new ArgumentNullException(nameof(tag));

  public bool Matches(UnitId unitId) => _tag.Matches(unitId.Tag);
}