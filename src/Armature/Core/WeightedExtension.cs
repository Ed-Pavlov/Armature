namespace Armature.Core
{
  public static class WeightedExtension
  {
    public static Weighted<IBuildAction> WithWeight(this IBuildAction entity, int weight)
    {
      return new Weighted<IBuildAction>(weight, entity);
    }

    public static Weighted<T> AddWeight<T>(this Weighted<T> weighted,  int weight)
    {
      return new Weighted<T>(weight + weighted.Weight, weighted.Entity);
    }
  }
}