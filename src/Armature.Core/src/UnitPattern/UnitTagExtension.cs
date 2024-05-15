using System;
using System.Runtime.CompilerServices;

namespace BeatyBit.Armature.Core;

public static class UnitTagExtension
{
  /// <summary>
  /// Checks if a <paramref name="patternTag"/> tag matches <paramref name="unitTag"/>, returns true if they are equal or the
  /// <paramref name="patternTag"/> is <see cref="Tag.Any"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Matches(this object? patternTag, object? unitTag)
  {
    if(ReferenceEquals(unitTag, Tag.Any))
      throw new ArgumentOutOfRangeException(
        nameof(unitTag),
        $"Building unit's tag can't be '{nameof(Tag)}.{nameof(Tag.Any)}' tag. Check arguments order in the call of this method");

    return ReferenceEquals(patternTag, Tag.Any) || Equals(patternTag, unitTag);
  }
}