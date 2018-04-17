using System;

namespace Armature
{
  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field
  | AttributeTargets.Event)]
  public class InjectAttribute : Attribute
  {
    /// <summary>
    /// 
    /// </summary>
    public readonly object InjectionPointId;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="injectionPointId"></param>
    public InjectAttribute(object injectionPointId = null) => InjectionPointId = injectionPointId;
  }
}