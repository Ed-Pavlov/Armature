using System.Collections.Generic;
using Armature.Core;

namespace Tests.UnitTests.BuildActions;

public class BuildSessionMock : IBuildSession
{
  private readonly IEnumerable<UnitId> _buildSequence;

  public BuildSessionMock() { }
  public BuildSessionMock(BuildResult buildResult) => BuildResult = buildResult;
  public BuildSessionMock(IEnumerable<UnitId> buildSequence) => _buildSequence = buildSequence;

  public BuildResult         BuildResult   { get; set; }
  public IEnumerable<UnitId> BuildSequence => _buildSequence;

  public BuildResult                 BuildUnit(UnitId     unitId) => throw new System.NotImplementedException();
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => throw new System.NotImplementedException();
}