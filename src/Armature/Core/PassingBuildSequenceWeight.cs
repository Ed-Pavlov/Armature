using System.Diagnostics.CodeAnalysis;

namespace Armature.Core
{

  /// <summary>
  /// Weights which are added to the build action by build steps which matches a not last <see cref="UnitInfo"/> in
  /// the build sequence.
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class PassingBuildSequenceWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (protected for using in inheritors)
    protected const int Step = 100;

    public const int AnyUnit = 0 - Step;
    public const int WeakSequenceOpenGeneric = Step;
    public const int WeakSequence = WeakSequenceOpenGeneric + Step;
  }
}