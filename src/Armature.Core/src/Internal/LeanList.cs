using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Armature.Core;

/// <summary>
/// A list which doesn't allocate inner array if contains less than five items
/// </summary>
public class LeanList<T> : IList<T>
{
  private T?       _0;
  private T?       _1;
  private T?       _2;
  private T?       _3;
  private List<T>? _list;

  public LeanList(int capacity = 1)
  {
    if(capacity > 4)
      _list = new List<T>(capacity - 4);
  }

  public LeanList(ICollection<T> collection) => AddRange(collection);

  public int Count { get; private set; }

  public void Add(T item)
  {
    if(Count >= 4)
      List.Add(item);
    else
      SetItem(Count)(item);

    Count += 1;
  }

  public void AddRange(ICollection<T> items)
  {
    var toSkip = 0;
    if(Count < 4)
      foreach(var item in items)
      {
        if(Count > 4)
          break;

        Add(item);
        toSkip++;
      }

    List.AddRange(items.Skip(toSkip));
  }

  public T this[int index]
  {
    get
    {
      if(index < 0 || Count < index) throw new ArgumentOutOfRangeException(nameof(index));
      return GetItem(index);
    }

    set
    {
      if(index < 0 || Count < index) throw new ArgumentOutOfRangeException(nameof(index));
      SetItem(index)(value);
    }
  }

  public int  IndexOf(T    item)
  {
    for(var i = 0; i < Count; i++)
      if(Equals(item, GetItem(i)))
        return i;

    return -1;
  }

  public bool Contains(T item) => IndexOf(item) >= 0;

  public void Clear()
  {
    _0 = _1 = _2 = _3 = default;
    _list?.Clear();
    Count = 0;
  }

  public IEnumerator<T> GetEnumerator()
  {
    for(var i = 0; i < Count; i++)
      yield return GetItem(i);
  }

  public struct Enumerator : IEnumerator<T>
  {
    private readonly LeanList<T> _list;
    private          int         _index;
    private          bool        _isDisposed = false;
    private          T?          _current;

    public Enumerator(LeanList<T> list)
    {
      _list = list;
      Reset();
    }

    public void Dispose()
    {
      if(_isDisposed) return;
      _isDisposed = true;
    }

    private readonly void CheckDisposed()
    {
      if(_isDisposed)
        throw new ObjectDisposedException(nameof(Enumerator));
    }

    public bool MoveNext()
    {
      CheckDisposed();
      var hasNext = _index < _list.Count;
      if(hasNext)
        Current = _list[_index];

      _index += 1;
      return hasNext;
    }

    public void Reset()
    {
      CheckDisposed();
      _index  = 0;
      _current = default;
    }

    public T Current
    {
      readonly get
      {
        CheckDisposed();
        return _current ?? throw new InvalidOperationException("Call MoveNext first");
      }

      private set => _current = value;
    }

    object IEnumerator.Current => Current!;
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public bool IsReadOnly                        => false;
  public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();
  public bool Remove(T   item) => throw new NotSupportedException();

  private List<T> List => _list ??= new List<T>();

  private Action<T> SetItem(int index)
    => index switch
       {
         0 => value => _0 = value,
         1 => value => _1 = value,
         2 => value => _2 = value,
         3 => value => _3 = value,
         _ => value => _list![index - 4] = value
       };

  private T GetItem(int index)
    => index switch
       {
         0 => _0!,
         1 => _1!,
         2 => _2!,
         3 => _3!,
         _ => _list![index - 4]
       };

  public void Insert(int   index, T item) => throw new NotSupportedException();
  public void RemoveAt(int index) => throw new NotSupportedException();
}
