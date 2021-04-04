using System;

namespace Armature
{
  /// <summary>
  ///   Attribute is used to mark type member into which dependencies should be injected
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field
  | AttributeTargets.Event)]
  public class InjectAttribute : Attribute
  {
    /// <summary>
    ///   Id of the injection point. Can be used to distinguishing different points.
    /// </summary>
    public readonly object? InjectionPointId;

    /// <param name="injectionPointId">Id of the injection point. Can be used to distinguishing different points.</param>
    public InjectAttribute(object? injectionPointId = null) => InjectionPointId = injectionPointId;
  }
}