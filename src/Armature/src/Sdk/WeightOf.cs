using Armature.BuildActions;
using Armature.Core;
using Armature.UnitPatterns.UnitType;
using JetBrains.Annotations;

namespace Armature.Sdk;

/// <summary>
/// Inherit this class to extend enum pattern with custom weights if you extend Armature with your own build stack, unit, or injection point
/// patterns which require to re-balance the weighting system.
/// </summary>
[PublicAPI]
public class WeightOf : Core.WeightOf
{
  [PublicAPI]
  public class InjectionPoint
  {
    protected const byte Step = 10;

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

  /// <summary>
  /// Weights of <see cref="IUnitPattern"/> is about two orders of magnitude higher than weights of <see cref="InjectionPoint"/> in order to registrations like
  ///
  /// builder.GetOrAddNode(new IfFirstUnit(Unit.Of(typeof(string)), WeightOf.InjectionPoint.ByName))
  ///        .GetOrAddNode(new IfFirstUnit(new IsInheritorOf(typeof(BaseType)), WeightOf.UnitPattern.SubtypePattern))
  ///
  /// never "wins"
  ///
  /// builder.GetOrAddNode(new IfFirstUnit(Unit.Of(typeof(string)), WeightOf.InjectionPoint.ByType))
  ///        .GetOrAddNode(new IfFirstUnit(new Unit.Of(typeof(ChildType)), WeightOf.UnitPattern.ExactTypePattern))
  ///
  /// because the second one is narrower case than the first one
  /// </summary>
  [PublicAPI]
  public class UnitPattern
  {
    protected static short Step { get; set; } = 10_000;

    /// <summary>
    /// Weight of type matched by open generic type, <see cref="IsGenericOfDefinition"/> unit pattern and <see cref="RedirectOpenGenericType"/> for details
    /// </summary>
    public static short OpenGenericPattern { get; protected set; } = Step;

    /// <summary>
    /// Weight of type matched by base type, <see cref="IsInheritorOf"/> unit pattern
    /// </summary>
    public static short SubtypePattern { get; protected set; } = (short) (OpenGenericPattern + Step);

    /// <summary>
    /// Weight of type matched by exact type
    /// </summary>
    public static short ExactTypePattern { get; protected set; } = (short) (SubtypePattern + Step);
  }
}
