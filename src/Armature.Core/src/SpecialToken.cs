﻿using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   These tokens are used by Armature framework to distinguish internal units to be built  from possible users for the same types
  /// </summary>
  public static class SpecialToken
  {
    /// <summary>
    ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
    /// </summary>
    public static readonly object Constructor = new UnitKey("Constructor");

    /// <summary>
    ///   Is used to build a <see cref="PropertyInfo" />
    /// </summary>
    public static readonly object Property = new UnitKey("Property");

    /// <summary>
    ///   Is used to build a value for inject point
    /// </summary>
    public static readonly object InjectValue = new UnitKey("InjectValue");

    public static bool IsSpecial(this object? obj) => obj is UnitKey token && (token == Constructor || token == Property || token == InjectValue);
  }
}
