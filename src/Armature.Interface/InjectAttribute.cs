using System;

namespace Armature.Interface
{
  [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Event)]
  public class InjectAttribute : Attribute
  {
    public readonly object InjectionPointId;

    public InjectAttribute(object injectionPointId = null)
    {
      InjectionPointId = injectionPointId;
    }
  }
}