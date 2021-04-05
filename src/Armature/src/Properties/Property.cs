namespace Armature
{
  public static class Property
  {
    /// <summary>
    ///   Adds a plan injecting dependencies into properties with corresponding <paramref name="names" />
    /// </summary>
    public static IPropertyValueBuildPlan Named(params string[] names) => new InjectPropertyByNameBuildPlan(names);

    /// <summary>
    ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="injectPointId" />
    /// </summary>
    public static IPropertyValueBuildPlan ByInjectPoint(params object[] injectPointId) => new InjectPropertyByInjectPointIdBuildPlan(injectPointId);
  }
}
