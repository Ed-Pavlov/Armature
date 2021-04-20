using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

// Resharper disable all

namespace Tests.Functional
{
  public class InjectDependenciesToPropertyTest
  {
    [Test]
    public void value_should_be_injected_into_injectpoint_property()
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target
       .GetOrAddNode(new SkipToLastUnit())
       .With( // add build action injecting values into property for any type
          skipToLastUnit =>
            skipToLastUnit
             .AddNode(new IfLastUnitMatches(CanBeInstantiatedPattern.Instance))
             .UseBuildAction(InjectIntoProperties.Instance, BuildStage.Initialize))
       .With( // add build action finding properties attributed with InjectAttribute for any type 
          skipToLastUnit =>
            skipToLastUnit
             .AddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
             .UseBuildAction(new GetPropertyListByInjectPointId(), BuildStage.Create));

      target.Treat<string>().AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void value_should_be_injected_into_property_by_name()
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(Property.Named(nameof(Subject.StringProperty))); // inject property adds a build action injecting values into property

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().BeNull();
      actual.StringProperty.Should().Be(expected);
    }

    [Test]
    public void value_should_be_injected_into_property_by_injectpointid([Values(null, Subject.InjectPointId)] object injectPointId)
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(
          Property.ByInjectPoint(
            injectPointId is null
              ? Empty<object>.Array
              : new[] {injectPointId})); // inject property adds a build action injecting values into property

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_use_provided_value_for_property_by_name()
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(ForProperty.Named(nameof(Subject.StringProperty)).UseValue(expected));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.StringProperty.Should().Be(expected);
      actual.InjectProperty.Should().BeNull();
    }

    [Test]
    public void should_use_provided_key_for_property_by_inject_point()
    {
      const string key      = "key";
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>(key).AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(ForProperty.WithInjectPoint(Subject.InjectPointId).UseKey(key));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_use_inject_point_id_as_key_for_property_by_inject_point()
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>(Subject.InjectPointId).AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(ForProperty.WithInjectPoint(Subject.InjectPointId).UseInjectPointIdAsKey());

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_use_resolver_for_property_by_type()
    {
      const int expected = 3254;

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(ForProperty.OfType<int>().UseFactoryMethod(_ => expected));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.IntProperty.Should().Be(expected);
    }

    public static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(GetLongestConstructor.Instance, BuildStage.Create),
             new IfLastUnitMatches(PropertyArgumentPattern.Instance)
              .UseBuildAction(new BuildArgumentForProperty(), BuildStage.Create)
           }
         };

    private class Subject
    {
      public const string InjectPointId = "id";

      [Inject(InjectPointId)]
      public string InjectProperty { get; set; }

      public string StringProperty { get; set; }

      public int IntProperty { get; set; }
    }
  }
}
