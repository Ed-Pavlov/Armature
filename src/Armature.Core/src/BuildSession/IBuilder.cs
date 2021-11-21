namespace Armature.Core;

public interface IBuilder
{
  BuildResult BuildUnit(UnitId unitId, IBuildChainPattern? auxPatternTree);
}