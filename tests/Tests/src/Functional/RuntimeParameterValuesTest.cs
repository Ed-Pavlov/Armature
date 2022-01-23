using System;
using System.IO;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class RuntimeParameterValuesTest
  {
    [Test]
    public void should_use_argument_of_exact_type()
    {
      const string expected = "megaString";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.StringCtor));

      // --act
      var actual = target.Build<Subject>(expected)!;

      // --assert
      actual.String1.Should().Be(expected);
    }

    [Test]
    public void should_use_argument_of_derived_type()
    {
      var expected = new MemoryStream();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.DisposableCtor));

      // --act
      var actual = target.Build<Subject>(expected)!;

      // --assert
      actual.Disposable.Should().Be(expected);
    }

    [Test]
    public void typed_and_named_parameters_should_work()
    {
      const string expectedString1 = "expected1";
      const string expectedString2 = "expected2";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target
       .Build<Subject>(
          ForParameter.OfType<string>().UseValue(expectedString1),
          ForParameter.Named("string2").UseValue(expectedString2))!;

      // --assert
      actual.String1.Should().Be(expectedString1);
      actual.String2.Should().Be(expectedString2);
    }

    [Test]
    public void pass_null_as_valid_value()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.StringCtor));

      var actual = target.Build<Subject>(ForParameter.OfType<string?>().UseValue(null))!;

      // --assert
      actual.String1.Should().BeNull();
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
             // inject into constructor
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create) // autowiring
         };

    [UsedImplicitly]
    private class Subject
    {
      public const string StringCtor     = "String";
      public const string DisposableCtor = "Disposable";

      public readonly IDisposable? Disposable;
      public readonly string?      String1;
      public readonly string       String2 = "bad";

      [Inject(StringCtor)]
      public Subject(string? string1) => String1 = string1;

      [Inject(DisposableCtor)]
      public Subject(IDisposable disposable) => Disposable = disposable;

      public Subject([Inject] string string1, string string2)
      {
        String1 = string1;
        String2 = string2;
      }
    }
  }
}