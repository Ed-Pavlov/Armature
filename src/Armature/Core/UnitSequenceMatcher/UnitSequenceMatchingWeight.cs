using System;
using System.Diagnostics.CodeAnalysis;

namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  /// Weights which are added to the build action by unit sequence matchers of certain kind.
  /// </summary>
  /// <remarks>In order to change default priority of matchers inherit this class and change values in static constructor.
  /// !!! Instantiate inherited class to ensure that static ctor is called !!!</remarks>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  public class UnitSequenceMatchingWeight
  {
    protected static int _step = 100;
    protected static int _lowest = 0;
    protected static int _any = _lowest - _step;
    protected static int _wildcard = _any + _step;
    protected static int _strict = _wildcard + _step;

    /// <summary>
    /// Is used for <see cref="AnyUnitSequenceMatcher" />
    /// </summary>
    public static int AnyUnit => _any;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public static int WildcardMatchingUnit => _wildcard;
    
    /// <summary>
    ///   Used for <see cref="StrictUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public static int StrictMatchingUnit => _strict;
  }
}