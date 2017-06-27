namespace Armature.Core
{
  public static class WeightedExtension
  {
    public static WeightedBuildAction WithWeight(this IBuildAction entity, int weight)
    {
      return new WeightedBuildAction(weight, entity);
    }
  }
}