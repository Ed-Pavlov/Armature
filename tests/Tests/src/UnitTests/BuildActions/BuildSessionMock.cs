using System;
using System.Collections.Generic;
using Armature.Core;

namespace Tests.UnitTests.BuildActions;

public class BuildSessionMock : IBuildSession
{
  private readonly IEnumerable<UnitId>? _buildChain;

  public BuildSessionMock() { }
  public BuildSessionMock(BuildResult buildResult) => BuildResult = buildResult;
  public BuildSessionMock(IEnumerable<UnitId> buildChain) => _buildChain = buildChain;

  public BuildResult         BuildResult   { get; set; }
  public IEnumerable<UnitId> BuildChain => _buildChain ?? throw new InvalidOperationException("Build chain is not initialized");

  public BuildResult                 BuildUnit(UnitId     unitId) => throw new NotSupportedException();
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => throw new NotSupportedException();
}