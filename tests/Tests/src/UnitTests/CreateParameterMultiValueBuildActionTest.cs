using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

// ReSharper disable UnusedParameter.Local

namespace Tests.UnitTests
{
  public class CreateParameterMultiValueBuildActionTest
  {
    [TestCaseSource(nameof(should_build_list_of_values_for_any_collection_cases))]
    public void should_build_list_of_values_for_any_collection(ParameterInfo parameterInfo)
    {
      // --arrange
      var target       = new BuildListArgumentForMethodParameter();
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.Stack).Returns(Unit.By(parameterInfo).ToBuildStack());
      A.CallTo(() => buildSession.BuildAllUnits(default, true)).WithAnyArguments().Returns(new[] {1, 2, 3}.Select(_ => new BuildResult(_).WithWeight(0)).ToList());

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.BuildResult.HasValue.Should().BeTrue();
      buildSession.BuildResult.Value.Should().BeOfType<List<int>>();
    }

    private static IEnumerable<ParameterInfo> should_build_list_of_values_for_any_collection_cases()
    {
      var type = typeof(TargetType);
      var ctor = type.GetConstructors()[0];

      var parameters = ctor.GetParameters();

      yield return parameters[0];
      yield return parameters[1];
      yield return parameters[2];
      yield return parameters[3];
      yield return parameters[4];
    }

    private class TargetType
    {
      public TargetType(
        IEnumerable<int>         enumerable,
        IReadOnlyCollection<int> readOnlyCollection,
        ICollection<int>         collection,
        IReadOnlyList<int>       readOnlyList,
        IList<int>               list) { }
    }
  }
}