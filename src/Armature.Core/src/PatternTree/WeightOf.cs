using System.Diagnostics.CodeAnalysis;

namespace Armature.Core
{
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class WeightOf
  {
    protected const short Step   = 1000;
    protected const short Lowest = Step * 20;

    public const short OpenGenericPattern = Lowest             + Step;
    public const short SubtypePattern     = OpenGenericPattern + Step;
    public const short StrictPattern      = SubtypePattern     + Step;

    public const int SkipToLastUnit = (Lowest - Step) << (sizeof(short) * 8);
    public const int FindUnit       = (Lowest + Step) << (sizeof(short) * 8);
    public const int FirstUnit      = FindUnit + (Step << (sizeof(short) * 8));
  }
}
