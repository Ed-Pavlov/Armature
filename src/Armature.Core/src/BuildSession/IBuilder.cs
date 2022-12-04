namespace Armature.Core;

public interface IBuilder
{
  BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxPatternTree);
}
