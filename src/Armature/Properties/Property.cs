namespace Armature
{
  public static class Property
  {
    public static IPropertyValueBuildPlan Named(params string[] names) => new InjectPropertyByNameBuildPlan(names);
    public static IPropertyValueBuildPlan ByInjectPoint(params object[] injectPointId) => new InjectPropertyByInjectPointIdBuildPlan(injectPointId);
  }
}