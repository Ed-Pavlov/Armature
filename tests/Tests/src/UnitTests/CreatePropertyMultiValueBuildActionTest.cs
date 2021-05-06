using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class CreatePropertyMultiValueBuildActionTest
  {
    [TestCaseSource(nameof(should_build_list_of_values_for_any_collection_cases))]
    public void should_build_list_of_values_for_any_collection(PropertyInfo propertyInfo)
    {
      // --arrange
      var target       = new BuildListArgumentForProperty();
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(propertyInfo, null)});

      A.CallTo(() => buildSession.BuildAllUnits(default))
       .WithAnyArguments()
       .Returns(new[] {1, 2, 3}.Select(_ => new BuildResult(_).WithWeight(0)).ToList());

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.BuildResult.HasValue.Should().BeTrue();
      buildSession.BuildResult.Value.Should().BeOfType<List<int>>();
    }

    private static IEnumerable<PropertyInfo> should_build_list_of_values_for_any_collection_cases()
    {
      var type       = typeof(TargetType);
      var properties = type.GetProperties();

      yield return properties[0];
      yield return properties[1];
      yield return properties[2];
      yield return properties[3];
      yield return properties[4];
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private class TargetType
    {
      public IEnumerable<int>         Enumerable         { get; set; }
      public IReadOnlyCollection<int> ReadOnlyCollection { get; set; }
      public ICollection<int>         Collection         { get; set; }
      public IReadOnlyList<int>       ReadOnlyList       { get; set; }
      public IList<int>               List               { get; set; }
    }
  }
}
