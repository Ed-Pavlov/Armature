using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   These keys are used by Armature to build such units as a constructor needed to instantiate an object,
  ///   or an argument for the method parameter and so on.
  /// </summary>
  /// <remarks>
  ///   No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, keys should be equal by the reference.
  /// </remarks>
  public class SpecialKey
  {
    private readonly string _name;
    public SpecialKey(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
    /// </summary>
    public static readonly object Constructor = new SpecialKey(nameof(Constructor));

    /// <summary>
    /// Is used to build a list of properties of a type
    /// </summary>
    public static readonly object PropertyList = new SpecialKey(nameof(PropertyList));

    /// <summary>
    ///   Is used to build an argument for inject point
    /// </summary>
    public static readonly object Argument = new SpecialKey(nameof(Argument));

    /// <summary>
    ///   Means "any key", it is used in patterns to match a unit regardless a key
    /// </summary>
    public static readonly SpecialKey Any = new(nameof(Any));

    /// <summary>
    ///   Is used to propagate key to building dependencies
    /// </summary>
    public static readonly SpecialKey Propagate = new(nameof(Propagate));

    public override string ToString() => _name;
  }

  public static class SpecialKeyExtension
  {
    public static object? GetKey(this object? buildActionKey, object? unitKey)
      => ReferenceEquals(buildActionKey, SpecialKey.Propagate) ? unitKey : buildActionKey;
  }
}
