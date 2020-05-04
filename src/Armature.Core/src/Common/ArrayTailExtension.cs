using System.Collections.Generic;
using System.Diagnostics;

namespace Armature.Core.Common
{
  public static class ArrayTailExtension
  {
    [DebuggerStepThrough]
    public static ArrayTail<T> GetTail<T>(this IList<T> array, int startIndex) => new ArrayTail<T>(array, startIndex);

    [DebuggerStepThrough]
    public static T Last<T>(this ArrayTail<T> arrayTail) => arrayTail[arrayTail.Length - 1];

    public static ArrayTail<T> AsArrayTail<T>(this IList<T> array) => new ArrayTail<T>(array, 0);
  }
}