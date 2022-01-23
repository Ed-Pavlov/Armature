using System;
using System.Runtime.CompilerServices;
using Armature.Core.Sdk;

namespace Armature.Core;

public static class UnitTagExtension
{
  /// <summary>
  /// Checks if <paramref name="pattern"/> matches <paramref name="unitTag"/>, returns true if they are equal of the
  /// <paramref name="pattern"/> is <see cref="SpecialTag.Any"/>
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Matches(this object? pattern, object? unitTag)
  {
    if(ReferenceEquals(unitTag, SpecialTag.Any))
      throw new ArgumentOutOfRangeException(
        nameof(unitTag),
        $"Building unit's tag can't be '{nameof(SpecialTag)}.{nameof(SpecialTag.Any)}' special tag. Check arguments order in the call of this method");

    return Equals(pattern, unitTag) || ReferenceEquals(pattern, SpecialTag.Any);
  }
}