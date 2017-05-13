using System.Linq;
using Armature;
using Armature.Interface;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable All

namespace Tests.Functional
{
  public class AttributedConstructorResolverTest
  {
    [Test(Description = "When .ctor marked with InjectAttribute has less parameters then not marked, FindAttributedConstructorBuildStep should have a priority")]
    public void AttributedCtor()
    {
      var expected = typeof(Subject).GetConstructors().First(_ => _.GetParameters().Length == 0);
      // --arrange
      var container = FunctionalTestHelper.CreateContainer();
      container
        .Treat<Subject>()
        .AsIs();

      // --act
      var instance = container.Build<Subject>();

      // --assert
      instance.AttributedConstructorIsCalled.Should().BeTrue();
    }

    class Subject
    {
      public const string IntCtorId = "int";

      public readonly bool AttributedConstructorIsCalled;

      [Inject]
      public Subject()
      {
        AttributedConstructorIsCalled = true;
      }

      public Subject(int i)
      {}
    }
  }
}