using Armature;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.Properties;
using Armature.Interface;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class InjectIntoPropertyTest
  {
    [Test]
    public void value_should_be_injected_into_injectpoint_property()
    {
      const string expected = "expectedString";
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
      target
        .AddOrGetUnitMatcher(new AnyUnitSequenceMatcher())
        .With( // add build action injecting values into property for any type
          anyMatcher =>
            anyMatcher
              .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(AnyTypeUnitMatcher.Instance, 0))
              .AddBuildAction(BuildStage.Initialize, InjectIntoPropertiesBuildAction.Instance))
        .With( // add build action finding properties attributed with InjectAttribute for any type 
          anyMatcher =>
            anyMatcher
              .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance, 0))
              .AddBuildAction(BuildStage.Create, new GetPropertyByInjectPointBuildAction()));

      target.Treat<string>().AsInstance(expected);
      target
        .Treat<SampleType>()
        .AsIs();

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void value_should_be_injected_into_property_by_name()
    {
      const string expected = "expectedString";
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<string>().AsInstance(expected);
      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(Property.ByName(nameof(SampleType.StringProperty))); // inject property adds a build action injecting values into property

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().BeNull();
      actual.StringProperty.Should().Be(expected);
    }
    
    [Test]
    public void value_should_be_injected_into_property_by_injectpointid([Values(null, SampleType.InjectPointId)] object injectPointId)
    {
      const string expected = "expectedString";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<string>().AsInstance(expected);
      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(
          injectPointId == null ? Property.ByInjectPoint() : Property.ByInjectPoint(injectPointId)); // inject property adds a build action injecting values into property

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_use_provided_value_for_property_by_name()
    {
      const string expected = "expectedString";
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(ForProperty.Named(nameof(SampleType.StringProperty)).UseValue(expected));

      // --act
      var actual = target.Build<SampleType>();
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
      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<string>(token).AsInstance(expected);
      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(ForProperty.WithInjectPoint(SampleType.InjectPointId).UseToken(token));

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_inject_point_id_as_token_for_property_by_inject_point()
    {
      const string expected = "expectedString";
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<string>(SampleType.InjectPointId).AsInstance(expected);
      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(ForProperty.WithInjectPoint(SampleType.InjectPointId).UseInjectPointIdAsToken());

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_resolver_for_property_by_type()
    {
      const int expected = 3254;
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<SampleType>()
        .AsIs()
        .InjectProperty(ForProperty.OfType<int>().UseResolver(_ => expected));

      // --act
      var actual = target.Build<SampleType>();
      actual.Should().NotBeNull();
      actual.IntProperty.Should().Be(expected);
    }

    private class SampleType
    {
      public const string InjectPointId = "id";
      [Inject(InjectPointId)]
      public string InjectProperty { get; set; }

      public string StringProperty { get; set; }
      
      public int IntProperty { get; set; }
    }
  }
}