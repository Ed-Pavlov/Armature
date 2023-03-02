﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Armature.Core;

/// <remarks>
/// No equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, these tags should be equal by the reference.
/// </remarks>
public class Tag : ILogString
{
  private readonly string _name;

  [PublicAPI]
  protected Tag([CallerMemberName] string name = "") => _name = name ?? throw new ArgumentNullException(nameof(name));

  /// <summary>
  /// Means "any tag", it is used in patterns to match an unit regardless a tag.
  /// </summary>
  public static readonly Tag Any = new();

  /// <summary>
  /// Is used to propagate a tag to building dependencies
  /// </summary>
  public static readonly Tag Propagate = new();

  [DebuggerStepThrough]
  public string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.QuoteIfNeeded()}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}

public static class TagExtension
{
  public static object? GetEffectiveTag(this object? patternTag, object? unitTag) => ReferenceEquals(patternTag, Tag.Propagate) ? unitTag : patternTag;
}