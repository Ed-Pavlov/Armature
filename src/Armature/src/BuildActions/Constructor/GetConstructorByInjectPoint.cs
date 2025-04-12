using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Gets the constructor of the type which is marked with <see cref="InjectAttribute" /> the optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />.
/// </summary>
public sealed record GetConstructorByInjectPoint : IBuildAction, ILogString
{
  private readonly BindingFlags     _bindingFlags;
  private readonly bool             _isAnyTag;
  private readonly HashSet<object?> _tags;

  /// <summary>
  /// Pass <see cref="Tag"/>.<see cref="Tag.Any"/> if you want that any constructor marked with <see cref="InjectAttribute"/>, regardless
  /// of what <see cref="InjectAttribute"/>.<see cref="InjectAttribute.Tag"/> is set, matches by this rule.
  /// </summary>
  [PublicAPI]
  public GetConstructorByInjectPoint(object? tag, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) : this(bindingFlags, tag) { }

  /// <summary>
  /// This constructor allows specifying multiple tags to find constructors marked with
  /// It provides only one constructor for a type, but can be used as a default rule for the set of Inject Point tags.
  /// </summary>
  /// <param name="bindingFlags">The binding flags to control visibility and instance/static scope of the constructor.</param>
  /// <param name="tags">An array of tags used to match constructors marked with <see cref="InjectAttribute" />.
  /// If empty and does not contain <see cref="Tag.Any" />, the rule does not match any.</param>
  [PublicAPI]
  public GetConstructorByInjectPoint(BindingFlags bindingFlags, params object?[] tags)
  {
    _bindingFlags = bindingFlags;
    _tags         = tags.ToHashSet();
    _isAnyTag     = _tags.Count == 0 || _tags.Contains(Tag.Any);
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();

    var constructors = unitType
                      .GetConstructors(_bindingFlags)
                      .Where(
                         ctor =>
                         {
                           var attributes = ctor.GetCustomAttributes<InjectAttribute>();
                           return _isAnyTag || attributes.Any(attribute => _tags.Contains(attribute.Tag));
                         })
                      .ToArray();

    if(constructors.Length > 1)
    {
      var exception = new ArmatureException(
        $"More than one constructors of the type {unitType.ToLogString()} are marked with attribute "
      + $"{nameof(InjectAttribute)} with one of specified tags: {_tags.ToHoconString()} ");

      for(var i = 0; i < constructors.Length; i++)
        exception.AddData($"Constructor #{i}", constructors[i]);

      throw exception;
    }

    var ctor = constructors.Length > 0 ? constructors[0] : null;
    ctor.WriteToLog(LogLevel.Trace);

    if(ctor is not null)
      buildSession.BuildResult = new BuildResult(constructors[0]);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => Hocon.Object<GetConstructorByInjectPoint>(("bindingFlags", _bindingFlags), ("tags", _tags));
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
