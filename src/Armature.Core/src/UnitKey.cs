using System;

namespace Armature.Core
{
  /// <summary>
  ///   No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, tokens should be equal by reference.
  /// </summary>
  public class UnitKey
  {
    /// <summary>
    ///   Means "any token"
    /// </summary>
    public static readonly UnitKey Any = new("Any");

    /// <summary>
    ///   Used to propagate token to building dependencies
    /// </summary>
    public static readonly UnitKey Propagate = new("Propagate");

    private readonly string _name;
    public UnitKey(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public override string ToString() => string.Format("{0}({1})", nameof(UnitKey), _name);
  }
}
