using System.Collections.Generic;
using JetBrains.Annotations;

namespace Armature.Core
{
  public class UnitBuilder
  {
    private readonly BuildSession _buildSession;
      
    public UnitBuilder(UnitInfo unitInfo, IEnumerable<UnitInfo> buildSequence, BuildSession buildSession)
    {
      _buildSession = buildSession;
      UnitInfo = unitInfo;
      BuildSequence = buildSequence;
    }

    public readonly UnitInfo UnitInfo;
    public readonly IEnumerable<UnitInfo> BuildSequence;
    public BuildResult BuildResult;
      
    [CanBeNull]
    public BuildResult Build([NotNull] UnitInfo unitInfo)
    {
      return _buildSession.BuildUnit(unitInfo);
    }
  }
}