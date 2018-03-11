using System.Diagnostics;

namespace Armature.Common
{
  internal static class CollectionExtension
  {
    [DebuggerStepThrough]
    public static bool EqualsTo<T>(this T[] left, T[] right)
    {
      if (left.Length != right.Length) return false;

      for (var i = 0; i < left.Length; i++)
        if (!Equals(left[i], right[i]))
          return false;

      return true;
    }
  }
}