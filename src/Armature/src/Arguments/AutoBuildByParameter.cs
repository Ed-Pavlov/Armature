namespace Armature
{
  public static class AutoBuildByParameter
  {
    public static IArgumentTuner Type { get; } = new BuildArgumentByParameterTypeTuner();
  }
}
