using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Armature.Core.Sdk;

public static class ArrayTailExtension
{
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Last<T>(this ArrayTail<T> arrayTail) => arrayTail[arrayTail.Length - 1];

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ArrayTail<T> AsArrayTail<T>(this IList<T> array) => new(array, 0);
}