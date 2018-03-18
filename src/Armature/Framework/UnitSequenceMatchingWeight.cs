using System;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;

namespace Armature.Framework
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
    protected static int _weakOpenGeneric = _lowest + _step;
    protected static int _weak = _weakOpenGeneric + _step;

    /// <summary>
    /// Is used for <see cref="AnyUnitSequenceMatcher" />
    /// </summary>
    public static int AnyUnit => _any;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains an open generic <see cref="Type" />
    /// </summary>
    public static int WeakMatchingOpenGenericUnit => _weakOpenGeneric;

    /// <summary>
    ///   Used for <see cref="WildcardUnitSequenceMatcher" /> wich matches with a <see cref="UnitInfo" /> contains a <see cref="Type" />
    /// </summary>
    public static int WeakMatchingTypeUnit => _weak;
  }
}