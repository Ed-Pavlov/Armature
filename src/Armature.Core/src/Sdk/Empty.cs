using System.Collections;
using System.Collections.Generic;

namespace BeatyBit.Armature.Core.Sdk;

public static class Empty
{
  // ReSharper disable once InconsistentNaming
  private static readonly IEnumerator? _enumerator;
  public static readonly  IEnumerator  Enumerator = _enumerator ??= new EmptyEnumerator();

  private class EmptyEnumerator : IEnumerator
  {
    public bool    MoveNext() => false;
    public void    Reset()    { }
    public object? Current    => null;
  }
}

public static class Empty<T>
{
  // ReSharper disable once InconsistentNaming
  private static readonly T[]? _array;
  public static readonly  T[]  Array = _array ??= System.Array.Empty<T>();

  // ReSharper disable once InconsistentNaming
  private static readonly List<T>? _list;
  public static readonly  List<T>  List = _list ??= new List<T>(0);
}
