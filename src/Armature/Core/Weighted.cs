using System;
using JetBrains.Annotations;

namespace Armature.Core
{
  public struct Weighted<T>
  {
    public readonly int Weight;
    public readonly T Entity;

    public Weighted(int weight, [NotNull] T entity)
    {
      if (entity == null) throw new ArgumentNullException("entity");
      Weight = weight;
      Entity = entity;
    }

    public override string ToString()
    {
      return string.Format("Weight={0}, Of={1}", Weight, Entity);
    }
  }
}