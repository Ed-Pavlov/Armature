using System;
using System.Diagnostics.CodeAnalysis;

namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Weights which are added to the build action by unit sequence matchers of certain kind.
  /// </summary>
  /// <remarks>
  ///   In order to change default priority of matchers inherit this class and change values in static constructor.
  ///   !!! Instantiate inherited class to ensure that static ctor is called !!!
  /// </remarks>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  public class UnitSequenceMatchingWeight
  {
    protected static int Step                = 100_000;
    protected static int Lowest              = 0;
    protected static int Any                 = Lowest              - Step * 10;
    protected static int WildcardOpenGeneric = Lowest              + Step;
    protected static int WildcardBaseType    = WildcardOpenGeneric + Step;
    protected static int Wildcard            = WildcardBaseType    + Step;
    protected static int Strict              = Wildcard            + Step;

    /// <summary>
    ///   Is used for <see cref="AnyUnitSequenceMatcher" />
    /// </summary>
    public static int AnyUnit => Any;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> which matches with a <see cref="UnitInfo" /> contains open generic type
    /// </summary>
    public static int WildcardMatchingOpenGenericUnit => WildcardOpenGeneric;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> which matches with a <see cref="UnitInfo" /> contains inheritors of a type
    /// </summary>
    public static int WildcardMatchingBaseTypeUnit => WildcardBaseType;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> which matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public static int WildcardMatchingUnit => Wildcard;

    /// <summary>
    ///   Used for <see cref="StrictUnitSequenceMatcher" /> which matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public static int StrictMatchingUnit => Strict;
  }
}
