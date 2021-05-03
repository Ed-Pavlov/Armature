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
    protected const byte Step   = 10;
    protected const byte Lowest = 20;

    public const byte OpenGenericPattern = Lowest             + Step;
    public const byte SubtypePattern     = OpenGenericPattern + Step;
    public const byte StrictPattern      = SubtypePattern     + Step;

    public const short SkipToLastUnit = (Lowest - Step) << (sizeof(byte) * 8);
    public const short FindUnit       = (Lowest + Step) << (sizeof(byte) * 8);
    public const short FirstUnit      = FindUnit + (Step << (sizeof(byte) * 8));
  }
}
