using System;

namespace Armature.Core
{
  /// <summary>
  ///   No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, keys should be equal by the reference.
  /// </summary>
  public class UnitKey
  {
    private readonly string _name;
    public UnitKey(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public override string ToString() => _name;
  }
}
