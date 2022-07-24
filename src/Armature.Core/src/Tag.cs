using Armature.Core.Sdk;

namespace Armature.Core;

public class Tag : SpecialTag
{
  private Tag(string name) : base(name) { }

  /// <summary>
  /// Means "any tag", it is used in patterns to match a unit regardless a tag
  /// </summary>
  public static readonly SpecialTag Any = new Tag(nameof(Any));
}