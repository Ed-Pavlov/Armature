using System;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;

namespace Armature.Framework
{
  /// <summary>
  ///   Weights which are added to the build action by build steps of certain kind. They are placed in one class to simplify the mantaining of consistency
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class UnitSequenceMatchingWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (protected for using in inheritors)
    protected const int Step = 100;

    /// <summary>
    ///   Is used for <see cref="AnyUnitSequenceMatcher" />
    /// </summary>
    public const int AnyUnit = 0 - Step;

    /// <summary>
    ///   Used for <see cref="WeakUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains an open generic <see cref="Type" />
    /// </summary>
    public const int WeakMatchingOpenGenericUnit = Step;

    /// <summary>
    ///   Used for <see cref="WeakUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public const int WeakMatchingTypeUnit = WeakMatchingOpenGenericUnit + Step;
  }
}