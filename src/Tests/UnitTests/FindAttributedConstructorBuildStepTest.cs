//using System;
//using System.Linq;
//using FluentAssertions;
//using NUnit.Framework;
//using Armature;
//using Armature.Interface;
//// ReSharper disable All
//
//namespace Tests.Unit
//{
//  public class AttributedConstructorResolverTest
//  {
//    [Test(Description = "When .ctor marked with InjectAttribute without injection point id")]
//    public void WithoutId()
//    {
//      var expected = typeof(Subject).GetConstructors().First(_ => _.GetParameters().Length == 0);
//      // --arrange
//      var resolver = new FindAttributedConstructorBuildStep();
//
//      // --act
//      var ctor = resolver.GetConstructorOf(typeof(Subject));
//
//      // --assert
//      ctor
//        .Should()
//        .BeSameAs(expected)
//        .And
//        .BeDecoratedWith<InjectAttribute>();
//    }
//
//    [Test(Description = "When .ctor marked with InjectAttribute with injection point id")]
//    public void WithId()
//    {
//      var expected = typeof(Subject).GetConstructors().First(_ => _.GetParameters().Length == 1 && _.GetParameters()[0].ParameterType == typeof(int));
//      // --arrange
//      var resolver = new FindAttributedConstructorBuildStep(Subject.IntCtorId);
//
//      // --act
//      var ctor = resolver.GetConstructorOf(typeof(Subject));
//
//      // --assert
//      ctor
//        .Should()
//        .BeSameAs(expected)
//        .And
//        .BeDecoratedWith<InjectAttribute>();
//    }
//
//    [Test(Description = "When .ctor marked with a custom attribute but InjectAttribute")]
//    public void WithCustomAttribute()
//    {
//      var expected = typeof(Subject).GetConstructors().First(_ => _.GetParameters().Length == 1 && _.GetParameters()[0].ParameterType == typeof(byte));
//      // --arrange
//      var resolver = new FindAttributedConstructorBuildStep(attribute => attribute is ObsoleteAttribute);
//
//      // --act
//      var ctor = resolver.GetConstructorOf(typeof(Subject));
//
//      // --assert
//      ctor
//        .Should()
//        .BeSameAs(expected)
//        .And
//        .BeDecoratedWith<ObsoleteAttribute>();
//    }
//
//    class Subject
//    {
//      public const string IntCtorId = "int";
//
//      [Inject]
//      public Subject()
//      {}
//
//      [Inject(IntCtorId)]
//      public Subject(int i)
//      {}
//
//      [Inject("string")]
//      public Subject(string str)
//      {}
//
//      [Obsolete]
//      public Subject(byte b)
//      {}
//    }
//  }
//}

