using System.Diagnostics.CodeAnalysis;

namespace Armature.Core
{
  /// <summary>
  /// Inherit this class to extend enum pattern with custom weights
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class WeightOf
  {
    public const byte Step   = 10;
    public const byte Lowest = Step * 2;

    public const byte OpenGenericPattern = Lowest             + Step;
    public const byte SubtypePattern     = OpenGenericPattern + Step;
    public const byte StrictPattern      = SubtypePattern     + Step;

    public const short SkipAll   = -10;
    public const short Match  = 10;
    // public const short SkipTillUnit  = 10;
    // public const short FirstUnit = SkipTillUnit; // FindUnit + (Step << (sizeof(byte) * 8));
  }
}
