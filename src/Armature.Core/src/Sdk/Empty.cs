using System.Collections.Generic;

namespace Armature.Core.Sdk;

public static class Empty<T>
{
  // ReSharper disable once InconsistentNaming
  private static readonly T[]? _array;
  public static readonly  T[]  Array = _array ??= System.Array.Empty<T>();

  // ReSharper disable once InconsistentNaming
  private static readonly List<T>? _list;
  public static readonly  List<T>  List = _list ??= new List<T>(0);
}