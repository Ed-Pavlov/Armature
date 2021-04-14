using System;

namespace Armature.Core
{
  /// <summary>
  ///   No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, keys should be equal by reference.
  /// </summary>
  public class UnitKey
  {
    /// <summary>
    ///   Means "any key"
    /// </summary>
    public static readonly UnitKey Any = new("Any");

    /// <summary>
    ///   Used to propagate key to building dependencies
    /// </summary>
    public static readonly UnitKey Propagate = new("Propagate");

    private readonly string _name;
    public UnitKey(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public override string ToString() => _name;
  }
}
