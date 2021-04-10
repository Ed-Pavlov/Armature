using System;

namespace Armature.Core
{
  public static class UnitKeyExtension
  {
    public static bool Matches(this object? matcherKey, object? unitKey)
    {
      if(ReferenceEquals(unitKey, UnitKey.Any))
        throw new ArgumentOutOfRangeException(
          nameof(unitKey),
          "Building unit key can't be 'Any' special key. Check arguments order in the call of this method");

      return Equals(matcherKey, unitKey) || Equals(matcherKey, UnitKey.Any);
    }
  }
}
