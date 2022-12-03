using System;
using System.Collections.Generic;
using System.Diagnostics;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using FluentAssertions;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace Tests.Performance
{
  // [Ignore("Run manually only")]
  public sealed class PerformanceTest
  {
    [Test]
    public void UnitPatternMatchesTest()
    {
      // 12.07.2022 00:00:06.2620682
      var patterns = new List<IUnitPattern>
                     {
                       new IsConstructor(),
                       new IsParameterInfoArray(),
                       new IsParameterInfo(),
                       new IsParameterAttributed(),
                       new IsPropertyInfoCollection(),
                       new IsPropertyInfo(),
                       new IsPropertyOfType(new UnitPattern(typeof(string)))
                     };

      for(var i = 0; i < 10_000; i++)
        patterns.Add(new UnitPattern(typeof(List<string>)));

      for(var i = 0; i < 1_000; i++)
        patterns.Add(new IsGenericOfDefinition(typeof(List<>), null));

      for(var i = 0; i < 1_000; i++)
        patterns.Add(new IsInheritorOf(typeof(IDisposable), null));

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsAssignableFromType(typeof(ITuner)));

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsPropertyInfo());

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsPropertyNamed("prop"));

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsParameterNamed("param"));

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsParameterAttributed());

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsPropertyAttributed());

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < 100_000; i++)
      {
        var unitId = new UnitId(i, "tag");

        foreach(var unitPattern in patterns)
          unitPattern.Matches(unitId);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
    }


    // There were 71994021 class of Equals on count == 3 000. Count of GetHashCode was exactly the count of IPatternTreeNode added into children collection
    // After the fix it becomes 42 of Equals and 23999 of GetHashCode
    [Test]
    public void AddOrGetNodeTest()
    {
      const int count = 25_000;

      var builder = new Builder("stage");

      for(var i = 0; i < count; i++)
      {
        var u1  = new UnitId(i, null);
        var u2  = new UnitId(i, i);
        var str = i.ToString();
        var u3  = new UnitId(str, null);
        var u4  = new UnitId(str, str);

        Treat(builder, u1).AsCreatedWith(() => null);
        Treat(builder, u2).AsInstance(null);
        Treat(builder, u3).AsCreatedWith(() => null);
        Treat(builder, u4).AsCreatedWith(() => null).AsSingleton();
      }

      Console.WriteLine(UnitPatternWrapper.EqualsCallsCount);
      Console.WriteLine(UnitPatternWrapper.GetHashCodeCallsCount);

      UnitPatternWrapper.EqualsCallsCount.Should().BeLessThan(1_000);
      UnitPatternWrapper.GetHashCodeCallsCount.Should().BeLessThan(250_000);
    }

    [Test]
    public void BuildUnitTest()
    {
      var builder = new Builder(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create);

      // Treat<I>().AsCreated<C>().AsSingleton();
      var registrationsCount = 3_000;
      var argsRegistrationCount = 100;

      for(var i = 1; i < registrationsCount; i++)
      {
        var i1         = i;
        var created    = (i1 * 1000).ToString();

        var @interface = () => new IfFirstUnit(new UnitPattern(i1));
        builder.AddNode(@interface()).UseBuildAction(new Redirect(new UnitId(created, null)), BuildStage.Create);

        builder.AddNode(new IfFirstUnit(new UnitPattern(created)))
               .AddNode(@interface())
               .UseBuildAction(new CreateWithFactoryMethod<string>(_ => created.ToString()), BuildStage.Create)
               .UseBuildAction(new Singleton(), BuildStage.Cache);
      }

      // Treat<I>().AsCreated<C>().UsingArguments(1, 2, 3)
      for(int k = 1; k < 4; k++)
      {
        var node = builder.AddNode(new IfFirstUnit(new UnitPattern("arg" + k)));

        for(int i = 1; i < argsRegistrationCount; i++)
        {
          var created = (i * 1000).ToString();
          node
           .AddNode(new IfFirstUnit(new UnitPattern(created)))
           .AddNode(new IfFirstUnit(new UnitPattern(i)))
           .UseBuildAction(new Instance<string>("arg" + k), BuildStage.Cache);
        }
      }

      Console.WriteLine("Registration finished");

      var sw = new Stopwatch();
      sw.Start();

      for(var i = 1; i < registrationsCount; i++)
      {
        var value = builder.BuildUnit(new UnitId(i, null));

        // value.Value.Should().Be((i * 1000).ToString());
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
    }

    private static BuildingTuner<object?> Treat(IBuildChainPattern pattern, UnitId unitId)
    {
      if(pattern is null) throw new ArgumentNullException(nameof(pattern));

      var unitPattern = new UnitPatternWrapper(new UnitPattern(unitId.Kind, unitId.Tag));
      return new BuildingTuner<object?>(new RootTuner(pattern), () => new SkipTillUnit(unitPattern), unitPattern);
    }

    private class UnitPatternWrapper : IUnitPattern
    {
      private readonly IUnitPattern _impl;

      public UnitPatternWrapper(IUnitPattern impl) => _impl = impl;

      public static long EqualsCallsCount      { get; private set; }
      public static long GetHashCodeCallsCount { get; private set; }

      public bool Matches(UnitId unitId) => _impl.Matches(unitId);

      private bool Equals(IUnitPattern? other)
      {
        EqualsCallsCount++;

        return _impl.Equals(other);
      }

      public override int GetHashCode()
      {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        GetHashCodeCallsCount++;

        return _impl.GetHashCode();
      }

      public override bool Equals(object? obj) => Equals(obj as UnitPattern);
    }

    private static Builder CreateTarget(Builder? parent = null)
      => new Builder
         {
           new IfFirstUnit(new IsConstructor())
            .UseBuildAction(
               new TryInOrder
               {
                 new GetConstructorByInjectPoint(),              // constructor marked with [Inject] attribute has more priority
                 Static.Of<GetConstructorWithMaxParametersCount>() // constructor with largest number of parameters has less priority
               },
               BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoArray())
            .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo())
            .UseBuildAction(
               new TryInOrder {Static.Of<BuildArgumentByParameterTypeAndTag>(), Static.Of<BuildArgumentByParameterType>()},
               BuildStage.Create),
           new IfFirstUnit(new IsPropertyInfo())
            .UseBuildAction(new TryInOrder {new BuildArgumentByPropertyType(), new BuildArgumentByPropertyTypeAndTag()}, BuildStage.Create)
         };
  }
}
