using System;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;


namespace Armature;

/// <summary>
/// Represents predefined build stages used by Armature framework. This list can be extended or completely replaced
/// if another framework is implemented on an Armature.Core base.
/// </summary>
/// <remarks>Use objects but int or enum in order to avoid memory traffic on boxing.</remarks>
/// <example>
/// new <see cref="Builder"/>(<see cref="BuildStage"/>.<see cref="BuildStage.Cache"/> , <see cref="BuildStage"/>.<see cref="Intercept"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Initialize"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Create"/>);
///
/// An action of the <see cref="BuildStage"/>.<see cref="BuildStage.Create"/> stage will create an Unit in the <see cref="IBuildAction"/>.<see cref="IBuildAction.Process"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="BuildStage.Initialize"/> stage if any will inject dependencies in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="BuildStage.Intercept"/> stage if any will subscribe events, log the event of creation, call methods, etc. in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="Cache"/> stage if any will cache the instance in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// </example>
public class BuildStage : ILogString
{
  /// <summary>
  /// Stage of intercepting any unit returned by the build process.
  /// </summary>
  public static readonly BuildStage Intercept = new("Intercept");

  /// <summary>
  /// Stage of building when already built and cached object can be reused.
  /// </summary>
  public static readonly BuildStage Cache = new("Cache");

  /// <summary>
  /// Stage of injecting dependencies into newly created unit.
  /// </summary>
  public static readonly BuildStage Initialize = new("Initialize");

  /// <summary>
  /// Stage of creating a unit, injects dependencies into a constructor, due it must be called to create the unit.
  /// </summary>
  public static readonly BuildStage Create = new("Create");

  private readonly string _name;

  [PublicAPI]
  protected BuildStage(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

  public          string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.ToHoconString()}";
  public override string ToString()      => ToHoconString();
}
