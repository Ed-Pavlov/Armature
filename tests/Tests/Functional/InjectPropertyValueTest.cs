using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Property;
using Armature.Core.Common;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class InjectPropertyValueTest
  {
    [Test]
    public void value_should_be_injected_into_injectpoint_property()
    {
      const string expected = "expectedString";
      // --arrange
      var target = CreateTarget();
      
      target
        .AddOrGetUnitSequenceMatcher(new AnyUnitSequenceMatcher())
        .With( // add build action injecting values into property for any type
          anyMatcher =>
            anyMatcher
              .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(AnyTypeMatcher.Instance))
              .AddBuildAction(BuildStage.Initialize, InjectIntoPropertiesBuildAction.Instance))
        .With( // add build action finding properties attributed with InjectAttribute for any type 
          anyMatcher =>
            anyMatcher
              .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
              .AddBuildAction(BuildStage.Create, new GetPropertyByInjectPointBuildAction()));

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
        .InjectProperty(Property.Named(nameof(Subject.StringProperty))); // inject property adds a build action injecting values into property

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
        .InjectProperty(
          Property.ByInjectPoint(injectPointId == null ? EmptyArray<object>.Instance : new[]{injectPointId})); // inject property adds a build action injecting values into property

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
        .InjectProperty(ForProperty.Named(nameof(Subject.StringProperty)).UseValue(expected));

      // --act
      var actual = target.Build<Subject>();
      
      // --assert
      actual.Should().NotBeNull();
      actual.StringProperty.Should().Be(expected);
      actual.InjectProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_provided_token_for_property_by_inject_point()
    {
      const string token = "token";
      const string expected = "expectedString";
      // --arrange
      var target = CreateTarget();

      target.Treat<string>(token).AsInstance(expected);
      target
        .Treat<Subject>()
        .AsIs()
        .InjectProperty(ForProperty.WithInjectPoint(Subject.InjectPointId).UseToken(token));

      // --act
      var actual = target.Build<Subject>();
      
      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_inject_point_id_as_token_for_property_by_inject_point()
    {
      const string expected = "expectedString";
      // --arrange
      var target = CreateTarget();

      target.Treat<string>(Subject.InjectPointId).AsInstance(expected);
      target
        .Treat<Subject>()
        .AsIs()
        .InjectProperty(ForProperty.WithInjectPoint(Subject.InjectPointId).UseInjectPointIdAsToken());

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
        .InjectProperty(ForProperty.OfType<int>().UseFactoryMethod(_ => expected));

      // --act
      var actual = target.Build<Subject>();
      
      // --assert
      actual.Should().NotBeNull();
      actual.IntProperty.Should().Be(expected);
    }

    public static Builder CreateTarget() => 
      new Builder(BuildStage.Initialize, BuildStage.Create)
    {
      new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(BuildStage.Create, GetLongesConstructorBuildAction.Instance),

        new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
          .AddBuildAction(BuildStage.Create, new CreatePropertyValueBuildAction()),
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