namespace Armature.Core;

public interface IBuilder
{
  /// <summary>
  /// Builds a unit identified by the <paramref name="unitId"/>.
  /// </summary>
  /// <param name="unitId">The id of the unit to be built.</param>
  /// <param name="auxBuildStackPatternTree">Optional tree of build stack patterns which will be used along with
  /// the permanent one to build a unit or it's dependencies.</param>
  /// <returns></returns>
  BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree);
}
