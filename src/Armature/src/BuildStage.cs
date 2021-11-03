using System;
using System.Diagnostics.CodeAnalysis;
using Armature.Core.Logging;


namespace Armature
{
  /// <summary>
  ///   Represents predefined build stages used by Armature framework. This list can be extended or completely replaced
  ///   if another framework is implemented on an Armature.Core base
  /// </summary>
  /// <remarks>Use objects but int or enum in order to avoid memory traffic on boxing</remarks>
  public class BuildStage : ILogString
  {
    /// <summary>
    ///   Stage of intercepting any unit returned by build process
    /// </summary>
    public static readonly BuildStage Intercept = new("Intercept");

    /// <summary>
    ///   Stage of building when already built and cached object can be reused
    /// </summary>
    public static readonly BuildStage Cache = new("Cache");

    /// <summary>
    ///   Stage of awareness that some unit was built
    /// </summary>
    public static readonly BuildStage Aware = new("Aware");

    /// <summary>
    ///   Stage of injecting dependencies into newly created unit
    /// </summary>
    public static readonly BuildStage Initialize = new("Initialize");

    /// <summary>
    ///   Stage of creating a unit, injects dependencies into a constructor, due it must be called to create the unit
    /// </summary>
    public static readonly BuildStage Create = new("Create");

    private readonly string _name;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected BuildStage(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public          string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.ToHoconString()}";
    public override string ToString()      => ToHoconString();
  }
}