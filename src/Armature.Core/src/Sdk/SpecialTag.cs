using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Core.Sdk;

/// <summary>
/// These tags are used by Armature to build such units as a constructor needed to instantiate an object,
/// or an argument for the method parameter and so on.
///
/// If you need to extend the set of special tags with your own, make a derived class and create tags using protected constructor.
/// </summary>
/// <remarks>
/// No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, tags should be equal by the reference.
/// </remarks>
public class SpecialTag : ILogString
{
  private readonly string _name;

  [PublicAPI]
  protected SpecialTag(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

  /// <summary>
  /// Is used to "build" a <see cref="ConstructorInfo" /> for a type
  /// </summary>
  public static readonly SpecialTag Constructor = new SpecialTag(nameof(Constructor));

  /// <summary>
  /// Is used to build a list of properties of a type
  /// </summary>
  public static readonly SpecialTag PropertyList = new SpecialTag(nameof(PropertyList));

  /// <summary>
  /// Is used to build an argument for the inject point
  /// </summary>
  public static readonly SpecialTag Argument = new SpecialTag(nameof(Argument));

  /// <summary>
  /// Means "any tag", it is used in patterns to match a unit regardless a tag
  /// </summary>
  public static readonly SpecialTag Any = new(nameof(Any));

  /// <summary>
  /// Is used to propagate a tag to building dependencies
  /// </summary>
  public static readonly SpecialTag Propagate = new(nameof(Propagate));

  [DebuggerStepThrough]
  public string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.QuoteIfNeeded()}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}

public static class SpecialTagExtension
{
  public static object? GetEffectiveTag(this object? patternTag, object? unitTag) => ReferenceEquals(patternTag, SpecialTag.Propagate) ? unitTag : patternTag;
}
