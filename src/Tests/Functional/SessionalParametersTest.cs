using System.IO;
using Armature;
using Armature.Interface;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class SessionalParametersTest
  {
    [Test]
    public void should_use_sessional_parameter_of_exact_type()
    {
      const string expected = "megastring";
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneStringCtorClass>()
        .AsIs();

      // --act
      var actual = target.Build<OneStringCtorClass>(expected);

      // --assert
      actual.Text.Should().Be(expected);
    }

    [Test]
    public void should_use_sessional_parameter_of_derived_type()
    {
      var expected = new MemoryStream();

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneDisposableCtorClass>()
        .AsIs();

      // --act
      var actual = target.Build<OneDisposableCtorClass>(expected);

      // --assert
      actual.Disposable.Should().Be(expected);
    }

    [Test]
    public void typed_and_named_parameters_should_work()
    {
      const string expectedString1 = "dslfj";
      const string expectedString2 = "l;kjsf";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<Subject>()
        .AsIs();

      // --act
      var actual = target.Build<Subject>(
        For.ParameterId(null).UseValue(expectedString1),
        For.ParameterName("string2").UseValue(expectedString2));

      // --assert
      actual.String1.Should().Be(expectedString1);
      actual.String2.Should().Be(expectedString2);
    }

    [Test]
    public void pass_null_as_valid_value()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneStringCtorClass>()
        .AsIs();

      // --act
      var actual = target.Build<OneStringCtorClass>(For.Parameter<string>().UseValue(null));

      // --assert
      actual.Text.Should().BeNull();
    }

    [UsedImplicitly]
    private class Subject
    {
      public readonly string String1;
      public readonly string String2;

      public Subject([Inject] string string1, string string2)
      {
        String1 = string1;
        String2 = string2;
      }
    }
  }
}