using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Armature.Core;

namespace Tests.Util;

public static class MiscExtension
{

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static BuildChain ToBuildChain(this IReadOnlyList<UnitId> array) => new(array, 0);
}