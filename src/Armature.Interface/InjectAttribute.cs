using System;
using JetBrains.Annotations;

namespace Armature.Interface
{
  [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Event)]
  public class InjectAttribute : Attribute
  {
    [CanBeNull]
    public readonly object InjectionPointId;

    public InjectAttribute([CanBeNull] object injectionPointId = null)
    {
      InjectionPointId = injectionPointId;
    }
  }
}