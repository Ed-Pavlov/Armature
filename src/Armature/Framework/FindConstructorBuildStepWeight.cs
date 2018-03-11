using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class FindConstructorBuildStepWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (introduced to use in inheritors)
    protected const int Step = 100;

    public const int Lowest = 0;
    public const int Attributed = Lowest + Step;
  }
}