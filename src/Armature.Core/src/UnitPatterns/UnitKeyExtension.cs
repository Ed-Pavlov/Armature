using System;

namespace Armature.Core
{
  public static class UnitKeyExtension
  {
    /// <summary>
    /// Checks if <paramref name="pattern"/> matches <paramref name="unitKey"/>, returns true if they are equal of the
    /// <paramref name="pattern"/> is <see cref="SpecialKey.Any"/>
    /// </summary>
    public static bool Matches(this object? pattern, object? unitKey)
    {
      if(ReferenceEquals(unitKey, SpecialKey.Any))
        throw new ArgumentOutOfRangeException(
          nameof(unitKey),
          $"Building unit's key can't be '{nameof(SpecialKey)}.{nameof(SpecialKey.Any)}' special key. Check arguments order in the call of this method");

      return Equals(pattern, unitKey) || ReferenceEquals(pattern, SpecialKey.Any);
    }
  }
}
