using System;
using System.Runtime.CompilerServices;
using BeatyBit.Armature.Core;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Represents predefined build stages used by Armature framework.
/// You are free to create your own build stages which fulfill your project needs.
///
/// Read the project's wiki to learn basic concepts of Armature.
/// </summary>
/// <remarks>Use objects but value types (int, struct) or enum to avoid memory traffic on boxing.</remarks>
/// <example>
/// new <see cref="Builder"/>(<see cref="BuildStage"/>.<see cref="Intercept"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Cache"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Initialize"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Create"/>);
/// </example>
public class BuildStage : ILogString
{
  /// <summary>
  /// Stage of building when already built and cached object can be reused.
  /// </summary>
  public static readonly BuildStage Cache = new();

  /// <summary>
  /// Stage of injecting dependencies into newly created unit.
  /// </summary>
  public static readonly BuildStage Initialize = new();

  /// <summary>
  /// Stage of creating a unit, injects dependencies into a constructor, due it must be called to create the unit.
  /// </summary>
  public static readonly BuildStage Create = new();

  /// <summary>
  /// Stage of processing fully initialized object, e.g., subscribe events or add it to object pool, etc.
  /// </summary>
  public static readonly BuildStage Process = new();

  /// <summary>
  /// Stage of intercepting unit returned by the build process for any reason, e.g., for wrapping it with another object.
  /// </summary>
  public static readonly BuildStage Intercept = new();

  /// <summary>
  /// Stage for notifying any concerned party that object was created, initialized, wrapped, and so on and so forth, and ready to work.
  /// </summary>
  public static readonly BuildStage Aware = new();

  /// <summary>
  /// Stage to perform some operations with <see cref="Core.Log"/> for diagnostic purpose.
  /// </summary>
  public static readonly BuildStage Log = new();

  private readonly string _name;

  [PublicAPI]
  protected BuildStage([CallerMemberName] string name = "") => _name = name ?? throw new ArgumentNullException(nameof(name));

  public          string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.ToHoconString()}";
  public override string ToString()      => ToHoconString();
}
