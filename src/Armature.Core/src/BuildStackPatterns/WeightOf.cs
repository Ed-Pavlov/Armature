using JetBrains.Annotations;

namespace BeatyBit.Armature.Core;

/// <summary>
/// Inherit this class to extend enum pattern with custom weights or want to re-balance the weighting system.
/// </summary>
[PublicAPI]
public class WeightOf
{
  /// <summary>
  /// Note that <see cref="SkipTillUnit"/> and <see cref="SkipWhileUnit"/> are multipliers, see the implementation of method
  /// <see cref="IBuildStackPattern"/>.<see cref="IBuildStackPattern.GatherBuildActions"/> in classes <see cref="Armature.Core.SkipTillUnit"/> and
  /// <see cref="Armature.Core.SkipWhileUnit"/>, whereas <see cref="IfFirstUnit"/> is an absolute value.
  ///
  /// Make sure that <see cref="SkipTillUnit"/> and <see cref="SkipWhileUnit"/> will never "win" <see cref="IfFirstUnit"/> when change their values.
  /// </summary>
  [PublicAPI]
  public class BuildStackPattern
  {
    public static int SkipWhileUnit { get; protected set; } = 0;

    public static int SkipTillUnit { get; protected set; } = -10;

    /// Weights of <see cref="IfFirstUnit"/> is about two orders of magnitude higher than weights of <see cref="UnitPattern"/> in order to registrations like
    ///
    /// builder.GetOrAddNode(new IfFirstUnit(Unit.Of(typeof(string)), WeightOf.InjectionPoint.ByType))
    ///        .GetOrAddNode(new SkipTillUnit(new Unit.Of(typeof(ChildType)), WeightOf.UnitPattern.ExactTypePattern))
    ///
    /// never "wins"
    ///
    /// builder.GetOrAddNode(new IfFirstUnit(Unit.Of(typeof(string)), WeightOf.InjectionPoint.ByName))
    ///        .GetOrAddNode(new IfFirstUnit(new IsInheritorOf(typeof(BaseType)), WeightOf.UnitPattern.SubtypePattern))
    ///
    /// because the first one is registration for building "in context" of ChildType whereas the second one is "personal" registration of
    /// all inheritors of BaseType
    public static int IfFirstUnit { get; protected set; } = 1_000_000;
  }
}
