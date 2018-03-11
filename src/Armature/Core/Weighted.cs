using System.Diagnostics;

namespace Armature.Core
{
  public struct Weighted<T>
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
    public override string ToString() => string.Format("{0}, Weight={1:n0}", Entity, Weight);
  }
}