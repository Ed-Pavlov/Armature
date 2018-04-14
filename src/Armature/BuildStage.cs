using System;
using System.Diagnostics.CodeAnalysis;
using Resharper.Annotations;

namespace Armature
{
  /// <summary>
  ///   Represents predefined build stages used by out of the box Armature framework syntax sugar. This list can be extended or completely replaced
  ///   if another framework is implemented on an Armature core base
  /// </summary>
  /// <remarks>Use objects but int or enum in order to avoid memory trafiic on boxing</remarks>
  public class BuildStage
  {
    public static readonly BuildStage Cache = new BuildStage("Cache");
    public static readonly BuildStage Initialize = new BuildStage("Initialize");
    public static readonly BuildStage Create = new BuildStage("Create");

    private readonly string _name;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected BuildStage([NotNull] string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public override string ToString() => _name;
  }
}