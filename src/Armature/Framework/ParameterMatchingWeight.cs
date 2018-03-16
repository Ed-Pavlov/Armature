using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  /// <summary>
  /// Matching weight for different parameter matchers
  /// </summary>
  /// <remarks>In order to change default priority of matchers inherit this class and change values in static constructor.
  /// !!! Instantiate inherited class to ensure that static ctor is called !!!</remarks>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  public class ParameterMatchingWeight
  {
    protected static int _step = 100;
    protected static int _lowest = 0;
    protected static int _weakType = _lowest + _step;
    protected static int _strictType = _weakType + _step;
    protected static int _attribute = _strictType + _step;
    protected static int _name = _attribute + _step;
   
    public static int Lowest => _lowest;

    /// <summary>
    /// Weight of matcher matching value for parameter by assignability value to parameter.
    /// </summary>
    public static int WeakTypedParameter => _weakType;

    /// <summary>
    /// Weight of matcher matching value for parameter by strict type equality.
    /// </summary>
    public static int TypedParameter => _strictType;

    /// <summary>
    /// Weight of matcher matching parameter by attribute.
    /// </summary>
    public static int AttributedParameter => _attribute;

    /// <summary>
    /// Weight of matcher matching parameter by name
    /// </summary>
    public static int NamedParameter => _name;
  }
}