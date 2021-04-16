using System;

namespace Armature.Core
{
  public static class UnitKeyExtension
  {
    public static bool Matches(this object? pattern, object? unitKey)
    {
      if(ReferenceEquals(unitKey, UnitKey.Any))
        throw new ArgumentOutOfRangeException(
          nameof(unitKey),
          $"Building unit's key can't be '{nameof(UnitKey)}.{nameof(UnitKey.Any)}' special key. Check arguments order in the call of this method");

      return Equals(pattern, unitKey) || Equals(pattern, UnitKey.Any);
    }
  }
}
