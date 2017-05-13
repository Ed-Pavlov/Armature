using Armature;
using NUnit.Framework;

namespace Tests.Functional
{
  public class CachingTest
  {
    [Test]
    public void AsSingleton()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<EmptyCtorClass>()
        .AsIs()
        .AsSingleton();

      // --act
      var expected = target.Build<EmptyCtorClass>();
      var actual = target.Build<EmptyCtorClass>();

      // --assert
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void AsInstance()
    {
      // --arrange
      var expected = new EmptyCtorClass();
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<EmptyCtorClass>()
        .AsInstance(expected);

      // --act
      var actual = target.Build<EmptyCtorClass>();

      // --assert
      Assert.That(actual, Is.SameAs(expected));
    }
  }
}