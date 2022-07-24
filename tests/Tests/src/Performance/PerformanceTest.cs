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
  public sealed class PerformanceTest
  {
    [Test]
    [Ignore("Run manually only")]
    public void Measure()
    {
      // 12.07.2022 00:00:06.2620682
      var patterns = new List<IUnitPattern>
                     {
                       new IsConstructor(),
                       new IsParameterInfoList(),
                       new IsParameterInfo(),
                       new IsParameterMarkedWithAttribute(),
                       new IsPropertyList(),
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
        patterns.Add(new  IsMethodParameterNamed("param"));

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsParameterMarkedWithAttribute());

      for(var i = 0; i < 100; i++)
        patterns.Add(new IsPropertyMarkedWithAttribute());

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
    public void AddOrGetUnitTestMatcherTest()
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
                 new GetConstructorByInjectPointId(),              // constructor marked with [Inject] attribute has more priority
                 Static.Of<GetConstructorWithMaxParametersCount>() // constructor with largest number of parameters has less priority
               },
               BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoList())
            .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo())
            .UseBuildAction(
               new TryInOrder {Static.Of<BuildArgumentByParameterInjectPointId>(), Static.Of<BuildArgumentByParameterType>()},
               BuildStage.Create),
           new IfFirstUnit(new IsPropertyInfo())
            .UseBuildAction(new TryInOrder {new BuildArgumentByPropertyType(), new BuildArgumentByPropertyInjectPointId()}, BuildStage.Create)
         };
  }
}
