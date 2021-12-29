using System;
using System.Collections.Generic;
using Armature.Core;

namespace Tests;

public static class Comparer
{
  public static IEqualityComparer<BuildChain> OfArrayTail<T>() => new ArrayTailEqualityComparer<T>();
}

public class ArrayTailEqualityComparer<T> : IEqualityComparer<BuildChain>
{
  public bool Equals(BuildChain x, BuildChain y)
  {
    if(x.Length != y.Length) return false;

    for(var i = 0; i < x.Length; i++)
      if(!Equals(x[i], y[i]))
        return false;

    return true;
  }
  public int GetHashCode(BuildChain array)
  {
    var hash = HashCode.Combine(array.GetHashCode());
    foreach(var item in array)
      hash = HashCode.Combine(hash, item.GetHashCode());

    return hash;
  }
}