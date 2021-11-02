using Armature.Core;
using FakeItEasy;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions;

public class BuildActionTestBase
{
  protected IBuildSession BuildSessionMock;

  [SetUp]
  public void BeforeEachTest() => BuildSessionMock = A.Fake<IBuildSession>();
}