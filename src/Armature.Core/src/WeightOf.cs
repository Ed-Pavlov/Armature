using System.Diagnostics.CodeAnalysis;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Inherit this class to extend enum pattern with custom weights if you extend Armature with your own build chain, unit or injection point
/// patterns which require to re-balance the weighting system.
/// </summary>
/// <remarks>For common usage of Armature it's not needed.</remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class WeightOf
{
  protected static byte Step { get; set; } = 5;

  public class InjectionPoint
  {
    /// <summary>
    /// Weight of argument matched by assignability to a parameter/property.
    /// </summary>
    public static byte ByTypeAssignability { get; protected set; } = Step;

    /// <summary>
    /// Weight of injection point (method parameter or property) matched by strict equality of a parameter/property type.
    /// </summary>
    public static byte ByExactType { get; protected set; } = (byte) (ByTypeAssignability + Step);

    /// <summary>
    /// Weight of argument matched by an attribute used to mark a parameter/property.
    /// </summary>
    public static byte ByInjectPointId { get; protected set; } = (byte) (ByExactType + Step);

    /// <summary>
    /// Weight of argument matched by a parameter name.
    /// </summary>
    public static byte ByName { get; protected set; } = (byte) (ByInjectPointId + Step);
  }

  public class UnitPattern
  {
    /// <summary>
    /// Weight of type matched by open generic type, <see cref="IsGenericOfDefinition"/> unit pattern and <see cref="RedirectOpenGenericType"/> for details
    /// </summary>
    public static byte OpenGenericPattern { get; protected set; } = Step;

    /// <summary>
    /// Weight of type matched by base type, <see cref="IsInheritorOf"/> unit pattern
    /// </summary>
    public static byte SubtypePattern { get; protected set; } = (byte) (OpenGenericPattern + Step);

    /// <summary>
    /// Weight of type matched by exact type
    /// </summary>
    public static byte ExactTypePattern { get; protected set; } = (byte) (SubtypePattern + Step);
  }

  public class BuildContextPattern
  {
    /// <summary>
    /// Neutral weight, by default is used in <see cref="SkipTillUnit"/> and <see cref="SkipWhileUnit"/> build chain patterns, in order
    /// to not "longest" registration branch take over "shorter" one. All ambiguity should be resolved by user.
    /// </summary>
    /// <remarks>!!! BE CAREFUL !!! Changing this value could lead unpredictable result in which registrations take over which.
    /// Only for very advanced usage by users who knows Armature better than the author.</remarks>
    public static byte Neutral { get; protected set; } = Step;

    /// <summary>
    /// <see cref="Core.SkipAllUnits"/> build chain pattern is used to set "default" rules for any building unit if it doesn't
    /// have a specific registration.
    /// That is why by default its weight is set very low, but not to the minimal possible value to leave a gap for user's needs.
    /// </summary>
    public static short SkipAllUnits { get; protected set; } = -30_000;

    /// <summary>
    /// By default the weight of <see cref="IfFirstUnit"/> build chain pattern's weight is increased in order to registrations
    /// like
    /// builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///      .GetOrAddNode(new IfFirstUnit(new IsAssignableFromType(typeof(string))))
    /// // ....
    /// "win" registrations like
    ///
    /// builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///      .GetOrAddNode(new SkipTillUnit(new IsAssignableFromType(typeof(string))))
    /// // ....
    ///
    /// Because the first one is a "personal" registration whereas the second one will be applied to all units building
    /// in the context of "MyType".
    ///
    /// Note that provided sample is "synthetic" see <see cref="SpecialTag"/> and <see cref="SkipWhileUnit"/> for details.
    /// </summary>
    public static short IfFirstUnit { get; protected set; } = (short) (Neutral + Step);
  }
}