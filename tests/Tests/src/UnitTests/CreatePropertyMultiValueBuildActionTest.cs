using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Armature.BuildActions.Property;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests
{
  public class CreatePropertyMultiValueBuildActionTest
  {
    [Test]
    public void should_build_list_of_values_for_any_collection(
        [ValueSource(nameof(should_build_list_of_values_for_any_collection_cases))] PropertyInfo propertyInfo,
        [Values(null, "tag")]                                                       object?      tag)
    {
      // --arrange
      var target       = new BuildListArgumentForProperty(tag);
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.Stack).Returns(Unit.Of(propertyInfo, tag).ToBuildStack());

      A.CallTo(() => buildSession.BuildAllUnits(Unit.Of(propertyInfo, tag)))
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

    private class TargetType
    {
#pragma warning disable CS8618
      [UsedImplicitly]
      public IEnumerable<int>         Enumerable         { get; set; }
      [UsedImplicitly]
      public IReadOnlyCollection<int> ReadOnlyCollection { get; set; }
      [UsedImplicitly]
      public ICollection<int>         Collection         { get; set; }
      [UsedImplicitly]
      public IReadOnlyList<int>       ReadOnlyList       { get; set; }
      [UsedImplicitly]
      public IList<int>               List               { get; set; }
#pragma warning restore CS8618
    }
  }
}