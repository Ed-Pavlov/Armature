using System;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class TreatInheritorsOfTest
  {
    [Test]
    public void Test()
    {
      var target = CreateTarget();

      target.TreatInheritorsOf<Base>().UsingArguments("Base");
      target.Treat<C1>().AsIs();
      target.Treat<C2>().AsIs().UsingArguments("C2");


      var actual = target.Build<C2>()!;

      Console.WriteLine(actual.Value);
    }

    private class Base
    {
      protected Base(string value) => Value = value;
      public string Value { get; }
    }

    [UsedImplicitly]
    private class C1 : Base
    {
      public C1(string value) : base(value) { }
    }

    [UsedImplicitly]
    private class C2 : Base
    {
      public C2(string value) : base(value) { }
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new SkipAllUnits
           {
             new IfFirstUnit(new IsConstructor()) // inject into constructor
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
           }
         };
  }
}