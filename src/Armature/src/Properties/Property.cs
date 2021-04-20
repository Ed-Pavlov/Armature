namespace Armature
{
  //TODO: what is the difference with ForProperty
  public static class Property
  {
    /// <summary>
    ///   Adds a plan injecting dependencies into properties with corresponding <paramref name="names" />
    /// </summary>
    public static IPropertyId Named(params string[] names) => new InjectPropertyByNameBuildPlan(names);

    /// <summary>
    ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="injectPointId" />
    /// </summary>
    public static IPropertyId ByInjectPoint(params object[] injectPointId) => new InjectPropertyByInjectPointIdBuildPlan(injectPointId);
  }
}
