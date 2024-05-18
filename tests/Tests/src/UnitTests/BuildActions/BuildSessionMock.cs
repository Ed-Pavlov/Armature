using System;
using System.Collections.Generic;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;

namespace Tests.UnitTests.BuildActions;

public class BuildSessionMock : IBuildSession
{
  private readonly BuildSession.Stack? _buildStack;

  public BuildSessionMock() { }
  public BuildSessionMock(BuildResult        buildResult) => BuildResult = buildResult;
  public BuildSessionMock(BuildSession.Stack stack) => _buildStack = stack;

  public BuildResult        BuildResult { get; set; }
  public BuildSession.Stack Stack       => _buildStack ?? throw new InvalidOperationException("Build stack is not initialized");

  public BuildResult                 BuildUnit(UnitId     unitId, bool engageParentBuilders = true) => throw new NotSupportedException();
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, bool engageParentBuilders = true) => throw new NotSupportedException();
}
