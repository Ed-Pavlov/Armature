using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   These keys are used by Armature to build such units as a constructor needed to instantiate an object,
  ///   or an argument for the method parameter and so on.
  /// </summary>
  public static class SpecialKey
  {
    /// <summary>
    ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
    /// </summary>
    public static readonly object Constructor = new UnitKey(nameof(Constructor));

    /// <summary>
    /// Is used to build a list of properties of a type
    /// </summary>
    public static readonly object Property = new UnitKey(nameof(Property));

    /// <summary>
    ///   Is used to build an argument for inject point
    /// </summary>
    public static readonly object Argument = new UnitKey(nameof(Argument));

    /// <summary>
    ///   Means "any key", it is used in patterns to match a unit regardless a key
    /// </summary>
    public static readonly UnitKey Any = new(nameof(Any));

    /// <summary>
    ///   Is used to propagate key to building dependencies
    /// </summary>
    public static readonly UnitKey Propagate = new(nameof(Propagate));

    public static bool IsSpecial(this object? obj)
      => obj is UnitKey key && (key == Any || key == Constructor || key == Property || key == Argument || key == Propagate);
  }
}
