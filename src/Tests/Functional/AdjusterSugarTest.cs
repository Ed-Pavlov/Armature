using System.IO;
using Armature;
using NUnit.Framework;

namespace Tests.Functional
{
  public class AdjusterSugarTest
  {
    [Test(Description = "Register type using UsingParameters(value) construction")]
    public void ParameterByWeakType()
    {
      // --arrange
      var expected = new MemoryStream();
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<OneDisposableCtorClass>()
        .AsIs()
        .UsingParameters(expected);

      // --act
      var actual = target.Build<OneDisposableCtorClass>();
      
      // --assert
      Assert.That(actual.Disposable, Is.SameAs(expected));
    }

    [Test(Description = "Register type using UsingParameters(value1, value2) construction")]
    public void TwoParametersByWeakType()
    {
      // --arrange
      var expectedDisposable = new MemoryStream();
      var expectedString = "ldksjf";
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<TwoDisposableStringCtorClass>()
        .AsIs()
        .UsingParameters(expectedDisposable, expectedString);

      // --act
      var actual = target.Build<TwoDisposableStringCtorClass>();
      
      // --assert
      Assert.That(actual.Disposable, Is.SameAs(expectedDisposable));
      Assert.That(actual.String, Is.SameAs(expectedString));
    }

    [Test(Description = "Register two types using UsingParameters(value) construction for parameter with equal type")]
    public void TwoTypesWithSameParameterType()
    {
      // --arrange
      var expectedOne = new MemoryStream();
      var expectedTwo = new MemoryStream();
      var target = FunctionalTestHelper.CreateContainer();
      target.Treat<OneDisposableCtorClass>()
        .AsIs()
        .UsingParameters(expectedOne);

      target
        .Treat<TwoDisposableStringCtorClass>()
        .AsIs()
        .UsingParameters(expectedTwo, "str");

      // --act
      var actualOne = target.Build<OneDisposableCtorClass>();
      var actualTwo = target.Build<TwoDisposableStringCtorClass>();
      
      // --assert
      Assert.That(actualOne.Disposable, Is.SameAs(expectedOne));
      Assert.That(actualTwo.Disposable, Is.SameAs(expectedTwo));
    }

    [Test(Description = "Register two interfaces redirected into one class using different parameters in these registrations")]
    public void TwoRegistrationsWithDifferentParameters()
    {
      // --arrange
      var expected1 = new MemoryStream();
      var expected2 = new MemoryStream();
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<IDisposableValue1>()
        .As<OneDisposableCtorClass>()
        .UsingParameters(expected1);

      target
        .Treat<IDisposableValue2>()
        .As<OneDisposableCtorClass>()
        .UsingParameters(expected2);

      // --act
      var actual1 = target.Build<IDisposableValue1>();
      var actual2 = target.Build<IDisposableValue2>();
      
      // --assert
      Assert.That(actual1.Disposable, Is.SameAs(expected1));
      Assert.That(actual2.Disposable, Is.SameAs(expected2));
    }
  }
}