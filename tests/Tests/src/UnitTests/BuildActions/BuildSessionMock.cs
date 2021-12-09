using System;
using System.Collections.Generic;
using Armature.Core;

namespace Tests.UnitTests.BuildActions;

public class BuildSessionMock : IBuildSession
{
  private readonly BuildChain? _buildChain;

  public BuildSessionMock() { }
  public BuildSessionMock(BuildResult buildResult) => BuildResult = buildResult;
  public BuildSessionMock(BuildChain buildChain) => _buildChain = buildChain;

  public BuildResult       BuildResult { get; set; }
  public BuildChain BuildChain  => _buildChain ?? throw new InvalidOperationException("Build chain is not initialized");

  public BuildResult                 BuildUnit(UnitId     unitId) => throw new NotSupportedException();
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => throw new NotSupportedException();
}