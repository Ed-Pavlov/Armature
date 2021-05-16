using System;
using System.Collections;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class InjectDependenciesToPropertyTest
  {
    [TestCaseSource(nameof(test_case_source))]
    public void value_should_be_injected_into_property_by_name(Func<PatternTree, FinalTuner> tune)
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expected);

      tune(target)
       .InjectInto(Property.Named(nameof(Subject.StringProperty)));

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().BeNull();
      actual.StringProperty.Should().Be(expected);
    }

    [TestCaseSource(nameof(test_case_source))]
    public void should_use_argument_for_property_by_name(Func<PatternTree, FinalTuner> tune)
    {
      const string expected = "expectedString";
      const string bad      = expected + "bad";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(bad);

      tune(target)
       .UsingArguments(ForProperty.Named(nameof(Subject.StringProperty)).UseValue(expected));

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.StringProperty.Should().Be(expected);
      actual.InjectProperty.Should().BeNull();
    }
    
    // target.TreatAll().InjectInto(Property.ByInjectPoint()); //TODO: is it possible to make it working

    [TestCaseSource(nameof(test_case_source))]
    public void should_use_argument_for_property_by_type(Func<PatternTree, FinalTuner> tune)
    {
      const int expected = 3254;

      // --arrange
      var target = CreateTarget();

      tune(target)
       .UsingArguments(ForProperty.OfType<int>().UseFactoryMethod(_ => expected));

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.IntProperty.Should().Be(expected);
    }
    
    [Test]
    public void should_inject_into_property_by_injectpointid([Values(null, Subject.InjectPointId)] object injectPointId)
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .InjectInto(injectPointId is null ? Property.ByInjectPoint() : Property.ByInjectPoint(new[] { injectPointId }));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_inject_into_property_of_interface_by_injectpointid(
      [Values(null, Subject.InterfaceInjectPointId)] object injectPointId)
    {
      const string expected = "expectedString";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expected);

      target.Treat<ISubject>().AsCreated<Subject>();
      target.Treat<ISubject>().InjectInto(injectPointId is null ? Property.ByInjectPoint() : Property.ByInjectPoint(new[] { injectPointId }));

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    [Test]
    public void should_use_key_for_property_argument_by_inject_point()
    {
      const string key      = "key";
      const string expected = "expectedString";
      const string bad      = expected + "bad";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(bad);
      target.Treat<string>(key).AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingArguments(ForProperty.WithInjectPoint(Subject.InjectPointId).UseKey(key));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_key_for_interface_property_argument_by_inject_point()
    {
      const string key      = "key";
      const string expected = "expectedString";
      const string bad      = expected + "bad";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(bad);
      target.Treat<string>(key).AsInstance(expected);

      target.Treat<ISubject>().UsingArguments(ForProperty.WithInjectPoint(Subject.InterfaceInjectPointId).UseKey(key));
      target.Treat<ISubject>().AsCreated<Subject>();

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_inject_point_id_as_key_for_property_by_inject_point()
    {
      const string expected = "expectedString";
      const string bad      = expected + "bad";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(bad);
      target.Treat<string>(Subject.InjectPointId).AsInstance(expected);

      target
       .Treat<Subject>()
       .AsIs()
       .UsingArguments(ForProperty.WithInjectPoint(Subject.InjectPointId).UseInjectPointIdAsKey());

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }
    
    [Test]
    public void should_use_inject_point_id_as_key_for_interface_property_by_inject_point()
    {
      const string expected = "expectedString";
      const string bad      = expected + "bad";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(bad);
      target.Treat<string>(Subject.InterfaceInjectPointId).AsInstance(expected);

      target.Treat<ISubject>().UsingArguments(ForProperty.WithInjectPoint(Subject.InterfaceInjectPointId).UseInjectPointIdAsKey());
      target.Treat<ISubject>().AsCreated<Subject>();
      
      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Should().NotBeNull();
      actual.InjectProperty.Should().Be(expected);
      actual.StringProperty.Should().BeNull();
    }

    public static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static<GetConstructorWithMaxParametersCount>.Instance, BuildStage.Create),
             new IfFirstUnit(new IsPropertyInfo())
              .UseBuildAction(new BuildArgumentByPropertyType(), BuildStage.Create)
           }
         };

    private static IEnumerable test_case_source()
    {
      yield return new TestCaseData(new Func<PatternTree, FinalTuner>(tree => tree.Treat<ISubject>().AsCreated<Subject>())).SetName("Subject");

      yield return new TestCaseData(
        new Func<PatternTree, FinalTuner>(
          tree =>
          {
            tree.Treat<ISubject>().AsCreated<Subject>();
            return tree.Treat<ISubject>();
          })).SetName("ISubject");
    }

    private interface ISubject
    {
      [Inject(Subject.InterfaceInjectPointId)]
      string InjectProperty { get; set; }

      string StringProperty { get; set; }
      int    IntProperty    { get; set; }
    }

    private class Subject : ISubject
    {
      public const string InjectPointId          = "id";
      public const string InterfaceInjectPointId = Subject.InjectPointId + "Interface";

      [Inject(InjectPointId)]
      public string InjectProperty { get; set; }

      public string StringProperty { get; set; }

      public int IntProperty { get; set; }
    }
  }
}
