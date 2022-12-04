using System.Collections.Generic;
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests;

public static class Comparer
{
  public static IEqualityComparer<BuildSession.Stack> OfArrayTail<T>() => new ArrayTailEqualityComparer<T>();
}

public class ArrayTailEqualityComparer<T> : IEqualityComparer<BuildSession.Stack>
{
  public bool Equals(BuildSession.Stack x, BuildSession.Stack y)
  {
    if(x.Length != y.Length) return false;

    for(var i = 0; i < x.Length; i++)
      if(!Equals(x[i], y[i]))
        return false;

    return true;
  }
  public int GetHashCode(BuildSession.Stack array)
  {
    unchecked
    {
      var hash = array.GetHashCode();
      foreach(var item in array)
        hash ^= 397 * item.GetHashCode();

      return hash;
    }
  }
}