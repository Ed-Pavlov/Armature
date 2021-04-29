using System;

namespace Armature
{
  //TODO: what is the difference with ForProperty
  public static class Property
  {
    public static IInjectPointTuner OfType<T>() => OfType(typeof(T));
    
    public static IInjectPointTuner OfType(Type type) => new PropertyByType(type, InjectPointMatchingWeight.Hz);
    
    /// <summary>
    ///   Adds a plan injecting dependencies into properties with corresponding <paramref name="names" />
    /// </summary>
    public static IInjectPointTuner Named(params string[] names) => new PropertyListByName(names, InjectPointMatchingWeight.Hz);

    /// <summary>
    ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="injectPointId" />
    /// </summary>
    public static IInjectPointTuner ByInjectPoint(params object[] injectPointId) => new PropertyListByInjectPointId(injectPointId, InjectPointMatchingWeight.Hz);
  }
}
