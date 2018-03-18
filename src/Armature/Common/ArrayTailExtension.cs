using System.Collections.Generic;
using System.Diagnostics;

namespace Armature.Common
{
  public static class ArrayTailExtension
  {
    [DebuggerStepThrough]
    public static ArrayTail<T> GetTail<T>(this IList<T> array, int startIndex) => new ArrayTail<T>(array, startIndex);

    [DebuggerStepThrough]
    public static T Last<T>(this ArrayTail<T> arrayTail) => arrayTail[arrayTail.Length - 1];
  }
}