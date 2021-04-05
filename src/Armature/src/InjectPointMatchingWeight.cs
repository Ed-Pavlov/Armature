using System.Diagnostics.CodeAnalysis;

namespace Armature
{
  /// <summary>
  ///   Matching weight for different parameter matchers
  /// </summary>
  /// <remarks>
  ///   In order to change default priority of matchers inherit this class and change values in static constructor.
  ///   !!! Instantiate inherited class to ensure that static ctor is called !!!
  /// </remarks>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  public class InjectPointMatchingWeight
  {
    protected static int Step       = 100_000;
    protected static int Zero       = 0;
    protected static int WeakType   = Zero       + Step;
    protected static int StrictType = WeakType   + Step;
    protected static int Attribute  = StrictType + Step;
    protected static int Name       = Attribute  + Step;

    /// <summary>
    ///   Weight of matcher matching value for parameter by assignability value to parameter.
    /// </summary>
    public static int WeakTypedParameter => WeakType;

    /// <summary>
    ///   Weight of matcher matching value for parameter by strict type equality.
    /// </summary>
    public static int TypedParameter => StrictType;

    /// <summary>
    ///   Weight of matcher matching parameter by attribute.
    /// </summary>
    public static int AttributedParameter => Attribute;

    /// <summary>
    ///   Weight of matcher matching parameter by name
    /// </summary>
    public static int NamedParameter => Name;
  }
}
