using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Core.Sdk;

/// <summary>
///   These keys are used by Armature to build such units as a constructor needed to instantiate an object,
///   or an argument for the method parameter and so on.
///
///   If you need to extend the set of special keys with your own, make a derived class and create keys using protected constructor.
/// </summary>
/// <remarks>
///   No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, keys should be equal by the reference.
/// </remarks>
public class SpecialKey : ILogString
{
  private readonly string _name;

  [PublicAPI]
  protected SpecialKey(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

  /// <summary>
  ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
  /// </summary>
  public static readonly SpecialKey Constructor = new SpecialKey(nameof(Constructor));

  /// <summary>
  /// Is used to build a list of properties of a type
  /// </summary>
  public static readonly SpecialKey PropertyList = new SpecialKey(nameof(PropertyList));

  /// <summary>
  ///   Is used to build an argument for the inject point
  /// </summary>
  public static readonly SpecialKey Argument = new SpecialKey(nameof(Argument));

  /// <summary>
  ///   Means "any key", it is used in patterns to match a unit regardless a key
  /// </summary>
  public static readonly SpecialKey Any = new(nameof(Any));

  /// <summary>
  ///   Is used to propagate a key to building dependencies
  /// </summary>
  public static readonly SpecialKey Propagate = new(nameof(Propagate));

  [DebuggerStepThrough]
  public string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.QuoteIfNeeded()}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}

public static class SpecialKeyExtension
{
  public static object? GetEffectiveKey(this object? patternKey, object? unitKey)
    => ReferenceEquals(patternKey, SpecialKey.Propagate) ? unitKey : patternKey;
}