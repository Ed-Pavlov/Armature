namespace Armature.Core
{
  public struct Weighted<T>
  {
    public readonly T Entity;
    public readonly int Weight;

    public Weighted(T entity, int weight)
    {
      Entity = entity;
      Weight = weight;
    }
    
    public override string ToString()
    {
      return string.Format("{0}, Weight={1:n0}", Entity, Weight);
    }
  }
}