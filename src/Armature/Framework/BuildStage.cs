using System;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Use objects but int or enum in order to avoid memory trafiic on boxing
  /// </summary>
  public class BuildStage
  {
    public static readonly BuildStage Cache = new BuildStage("Cache");
    public static readonly BuildStage Redirect = new BuildStage("Redirect");
    public static readonly BuildStage Initialize = new BuildStage("Initialize");
    public static readonly BuildStage Create = new BuildStage("Create");

    private readonly string _name;
    private BuildStage([NotNull] string name)
    {
      if (name == null) throw new ArgumentNullException("name");
      _name = name;
    }

    public override string ToString()
    {
      return _name;
    }
  }
}