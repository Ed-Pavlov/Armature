using System;
using System.Runtime.CompilerServices;
using Armature.Core;
using JetBrains.Annotations;


namespace Armature;

/// <summary>
/// Represents predefined build stages used by Armature framework.
/// </summary>
/// <remarks>Use objects but int or enum in order to avoid memory traffic on boxing.</remarks>
/// <example>
/// new <see cref="Builder"/>(<see cref="BuildStage"/>.<see cref="Intercept"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Cache"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Initialize"/>, <see cref="BuildStage"/>.<see cref="BuildStage.Create"/>);
///
/// An action of the <see cref="BuildStage"/>.<see cref="BuildStage.Create"/> stage will create a unit in the <see cref="IBuildAction"/>.<see cref="IBuildAction.Process"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="BuildStage.Initialize"/> stage if any will inject dependencies in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="BuildStage.Intercept"/> stage if any will subscribe events, log the event of creation, call methods, etc. in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// Then an action of the <see cref="BuildStage"/>.<see cref="Cache"/> stage if any will cache the instance in the <see cref="IBuildAction"/>.<see cref="IBuildAction.PostProcess"/> method.
/// </example>
public class BuildStage : ILogString
{
  /// <summary>
  /// Stage to perform some operations with <see cref="Core.Log"/> for diagnostic purpose
  /// </summary>
  public static readonly BuildStage Log = new();

  /// <summary>
  /// Stage of intercepting any unit returned by the build process.
  /// </summary>
  public static readonly BuildStage Intercept = new();

  /// <summary>
  /// Stage of building when already built and cached object can be reused.
  /// </summary>
  public static readonly BuildStage Cache = new();

  /// <summary>
  /// Stage for notifying any concerned party that object was created
  /// </summary>
  public static readonly BuildStage Aware = new();

  /// <summary>
  /// Stage of processing fully initialized object, e.g. subscribe events or add it to object pool etc.
  /// </summary>
  public static readonly BuildStage Process = new();

  /// <summary>
  /// Stage of injecting dependencies into newly created unit.
  /// </summary>
  public static readonly BuildStage Initialize = new();


  /// <summary>
  /// Stage of creating a unit, injects dependencies into a constructor, due it must be called to create the unit.
  /// </summary>
  public static readonly BuildStage Create = new();

  private readonly string _name;

  [PublicAPI]
  protected BuildStage([CallerMemberName] string name = "") => _name = name ?? throw new ArgumentNullException(nameof(name));

  public          string ToHoconString() => $"{GetType().GetShortName().QuoteIfNeeded()}.{_name.ToHoconString()}";
  public override string ToString()      => ToHoconString();
}
