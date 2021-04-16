using System;
using System.Diagnostics.CodeAnalysis;

namespace Armature.Core
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
  public class QueryWeight
  {
    protected const int Step                = 100_000;
    protected const int Lowest              = 0;
    public const    int Any                 = Lowest              - Step * 10;
    protected const int WildcardOpenGeneric = Lowest              + Step;
    protected const int WildcardBaseType    = WildcardOpenGeneric + Step;
    protected const int Wildcard            = WildcardBaseType    + Step;
    protected const int Strict              = Wildcard            + Step;

    /// <summary>
    ///   Is used for <see cref="SkipToLastUnit" />
    /// </summary>
    public static int AnyUnit => Any;

    /// <summary>
    ///   Used for <see cref="FindFirstUnit" /> which matches with a <see cref="UnitId" /> contains open generic type
    /// </summary>
    public static int WildcardMatchingOpenGenericUnit => WildcardOpenGeneric;

    /// <summary>
    ///   Used for <see cref="FindFirstUnit" /> which matches with a <see cref="UnitId" /> contains inheritors of a type
    /// </summary>
    public static int WildcardMatchingBaseTypeUnit => WildcardBaseType;

    /// <summary>
    ///   Used for <see cref="FindFirstUnit" /> which matches with a <see cref="UnitId" /> contains a <see cref="Type" />
    /// </summary>
    public static int WildcardMatchingUnit => Wildcard;

    /// <summary>
    ///   Used for <see cref="IfFirstUnitIs" /> which matches with a <see cref="UnitId" /> contains a <see cref="Type" />
    /// </summary>
    public static int StrictMatchingUnit => Strict;
  }
}
