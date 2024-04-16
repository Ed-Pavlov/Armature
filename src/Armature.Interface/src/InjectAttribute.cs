using System;

namespace Armature;

/// <summary>
/// Attribute is used to mark type member into which dependencies should be injected
/// </summary>
[AttributeUsage(
  AttributeTargets.Constructor
| AttributeTargets.Method
| AttributeTargets.Property
| AttributeTargets.Parameter
| AttributeTargets.Field
| AttributeTargets.Event,
  AllowMultiple = true)]
public class InjectAttribute : Attribute
{
  /// <summary>
  /// Optional tag of the injection point, can be used by build actions.
  /// </summary>
  public readonly object? Tag;

  /// <param name="tag">Optional tag of the injection point, can be used by build actions.</param>
  public InjectAttribute(object? tag = null) => Tag = tag;
}
