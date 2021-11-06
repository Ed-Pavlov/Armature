using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core;

namespace Tests.Common
{
  public static class ArrayTailExtension
  {
    [DebuggerStepThrough]
    public static ArrayTail<T> GetTail<T>(this IList<T> array, int startIndex) => new(array, startIndex);
  }
}
