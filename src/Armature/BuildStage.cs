using System;
using System.Diagnostics.CodeAnalysis;
using Resharper.Annotations;

namespace Armature
{
  /// <summary>
  ///   Represents predefined build stages used by Armature framework. This list can be extended or completely replaced
  ///   if another framework is implemented on an Armature.Core base
  /// </summary>
  /// <remarks>Use objects but int or enum in order to avoid memory trafiic on boxing</remarks>
  public class BuildStage
  {
    /// <summary>
    /// Stage of building when already built and cached object can be reused
    /// </summary>
    public static readonly BuildStage Cache = new BuildStage("Cache");
    
    /// <summary>
    /// Stage of injecting dependencies into newly created unit
    /// </summary>
    public static readonly BuildStage Initialize = new BuildStage("Initialize");
    
    /// <summary>
    /// Stage of creating an unit, injects dependencies into a constructor, due it must be called to create the unit 
    /// </summary>
    public static readonly BuildStage Create = new BuildStage("Create");

    private readonly string _name;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected BuildStage([NotNull] string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public override string ToString() => _name;
  }
}