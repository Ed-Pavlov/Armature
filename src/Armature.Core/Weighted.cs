using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  public struct Weighted<T> : IComparable<Weighted<T>>
  {
    public readonly T Entity;
    public readonly int Weight;

    [DebuggerStepThrough]
    public Weighted(T entity, int weight)
    {
      Entity = entity;
      Weight = weight;
    }

    [DebuggerStepThrough]
    public int CompareTo(Weighted<T> other) => Weight.CompareTo(other.Weight);

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}, Weight={1:n0}", Entity.ToLogString(), Weight);
  }
}