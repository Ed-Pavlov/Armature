using System.Diagnostics.CodeAnalysis;

namespace Armature
{
  /// <summary>
  ///   Matching weight for different parameter matchers
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class WeightOfArgument
  {
    public const short Step   = 5;

    /// <summary>
    ///   Weight of argument matched by assignability to a parameter/property.
    /// </summary>
    public const short ByTypeAssignability = Step;

    /// <summary>
    ///   Weight of argument matched by strict equality of a parameter/property type.
    /// </summary>
    public const short ByType = ByTypeAssignability + Step;

    /// <summary>
    ///   Weight of argument matched by an attribute used to mark a parameter/property.
    /// </summary>
    public const short ByInjectPointId = ByType + Step;

    /// <summary>
    ///   Weight of argument matched by a parameter name.
    /// </summary>    
    public const short ByName = ByInjectPointId + Step;
  }
}
