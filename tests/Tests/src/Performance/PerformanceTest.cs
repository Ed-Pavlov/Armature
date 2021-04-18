using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Armature;
using Armature.Core;
using FluentAssertions;
using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Kernel;
using NUnit.Framework;

namespace Tests.Performance
{
  public sealed class PerformanceTest
  {
    private readonly List<Type> _types = new()
                                         {
                                           typeof(bool),
                                           typeof(byte),
                                           typeof(char),
                                           typeof(short),
                                           typeof(ushort),
                                           typeof(int),
                                           typeof(uint),
                                           typeof(long),
                                           typeof(ulong),
                                           typeof(float),
                                           typeof(double),
                                           typeof(decimal),
                                           typeof(string),
                                           typeof(bool[]),
                                           typeof(byte[]),
                                           typeof(char[]),
                                           typeof(short[]),
                                           typeof(ushort[]),
                                           typeof(int[]),
                                           typeof(uint[]),
                                           typeof(long[]),
                                           typeof(ulong[]),
                                           typeof(float[]),
                                           typeof(double[]),
                                           typeof(decimal[]),
                                           typeof(string[])
                                         };

    [Test]
    [Ignore("Run manually only")]
    [DotMemoryUnit(SavingStrategy = SavingStrategy.OnCheckFail, CollectAllocations = true)]
    public void Measure()
    {
      var parentContainer = CreateTarget();
      var expected        = new MemoryStream();

      parentContainer
       .Treat<IDisposable>()
       .AsInstance(expected);

      var container = CreateTarget(parentContainer);

      container
       .Building<IDisposable>()
       .Building<MemoryStream>()
       .Treat<byte[]>()
       .AsInstance(new byte[1]);

      foreach(var type in _types)
        container
         .Building(type)
         .Treat<IDisposable>()
         .AsCreated<MemoryStream>()
         .UsingMethodArguments(0);


      var mem1 = dotMemory.Check();

      var sw = new Stopwatch();
      sw.Start();

      for(var i = 0; i < 100_000; i++)
      {
        var obj = container.Build<IDisposable>();
        Assert.AreSame(obj, expected);
      }

      sw.Stop();

      dotMemory.Check(memory => Console.WriteLine(memory.GetTrafficFrom(mem1)));
      dotMemoryApi.SaveCollectedData();


      Console.WriteLine(sw.Elapsed);
    }


    // There were 71994021 class of Equals on count == 3 000. Count of GetHashCode was exactly the count of IUnitSequenceMatchers added into children collection
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

      UnitPatternWrapper.EqualsCallsCount.Should().BeLessThan(1_000);
      UnitPatternWrapper.GetHashCodeCallsCount.Should().BeLessThan(250_000);
    }

    private static TreatingTuner Treat(IPatternTreeNode buildPlans, UnitId unitId)
    {
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      var unitMatcher = new UnitPatternWrapper(new Pattern(unitId.Kind, unitId.Key));

      var query = new FindUnitMatches(unitMatcher);

      return new TreatingTuner(buildPlans.GetOrAddNode(query));
    }

    private class UnitPatternWrapper : IUnitPattern
    {
      private readonly IUnitPattern _impl;

      public UnitPatternWrapper(IUnitPattern impl) => _impl = impl;

      public static long EqualsCallsCount      { get; private set; }
      public static long GetHashCodeCallsCount { get; private set; }

      public bool Matches(UnitId unitId) => _impl.Matches(unitId);

      public bool Equals(IUnitPattern other)
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

      public override bool Equals(object obj) => Equals(obj as Pattern);
    }

    private static Builder CreateTarget(Builder parent = null)
    {
      var treatAll = new SkipToLastUnit
                     {
                       // inject into constructor
                       new IfLastUnitMatches(ConstructorPattern.Instance)
                        .UseBuildAction(
                           BuildStage.Create,
                           new OrderedBuildActionContainer
                           {
                             new GetConstructorByInjectPointId(), // constructor marked with [Inject] attribute has more priority
                             GetLongestConstructor.Instance   // constructor with largest number of parameters has less priority
                           }),
                       new IfLastUnitMatches(MethodArgumentPattern.Instance)
                        .UseBuildAction(
                           BuildStage.Create,
                           new OrderedBuildActionContainer
                           {
                             BuildArgumentForMethodWithPointIdAsKey.Instance, BuildArgumentForMethodParameter.Instance
                           }),
                       new IfLastUnitMatches(PropertyArgumentPattern.Instance)
                        .UseBuildAction(
                           BuildStage.Create,
                           new OrderedBuildActionContainer {new BuildArgumentForProperty()})
                     };

      var buildStages = new object[] {BuildStage.Cache, BuildStage.Initialize, BuildStage.Create};
      var builder     = parent is null ? new Builder(buildStages) : new Builder(buildStages, parent);
      builder.Children.Add(treatAll);

      return builder;
    }
  }
}
