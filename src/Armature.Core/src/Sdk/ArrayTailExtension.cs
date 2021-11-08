using System.Collections.Generic;
using System.Diagnostics;

namespace Armature.Core.Sdk
{
  public static class ArrayTailExtension
  {
    [DebuggerStepThrough]
    public static T Last<T>(this ArrayTail<T> arrayTail) => arrayTail[arrayTail.Length - 1];

    [DebuggerStepThrough]
    public static ArrayTail<T> AsArrayTail<T>(this IList<T> array) => new(array, 0);
  }
}