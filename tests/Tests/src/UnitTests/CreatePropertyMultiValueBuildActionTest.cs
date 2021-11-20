﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class CreatePropertyMultiValueBuildActionTest
  {
    [Test]
    public void should_build_list_of_values_for_any_collection(
        [ValueSource(nameof(should_build_list_of_values_for_any_collection_cases))] PropertyInfo propertyInfo,
        [Values(null, "key")]                                                       object?      key)
    {
      // --arrange
      var target       = new BuildListArgumentForProperty(key);
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildChain).Returns(new[] {new UnitId(propertyInfo, key)});

      A.CallTo(() => buildSession.BuildAllUnits(new UnitId(propertyInfo, key)))
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