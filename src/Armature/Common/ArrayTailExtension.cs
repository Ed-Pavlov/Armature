using System.Collections.Generic;

namespace Armature.Common
{
  public static class ArrayTailExtension
  {
    public static ArrayTail<T> GetTail<T>(this IList<T> array, int startIndex)
    {
      return new ArrayTail<T>(array, startIndex);
    }

    public static T GetLastItem<T>(this ArrayTail<T> arrayTail)
    {
      return arrayTail[arrayTail.Length - 1];
    }
  }
}