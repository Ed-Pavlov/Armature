using System.Collections.Generic;

namespace BeatyBit.Armature.Core;

/// <summary>
/// This concept is introduced for increasing performance purposes.
/// If an implementation of this interface contains a simple pattern w/o any dynamic matching logic,
/// such a pattern can be placed in a <see cref="Dictionary{TKey,TValue}"/> instead of the Tree and be resolved faster.
/// </summary>
public interface IStaticPattern
{
  bool IsStatic(out UnitId unitId);
}
