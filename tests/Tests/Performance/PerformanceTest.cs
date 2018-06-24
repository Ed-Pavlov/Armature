﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Kernel;
using NUnit.Framework;

namespace Tests.Performance
{
  public sealed class PerformanceTest
  {
    private readonly List<Type> _types = new List<Type>
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
      var expected = new MemoryStream();

      parentContainer
        .Treat<IDisposable>()
        .AsInstance(expected);

      var container = CreateTarget(parentContainer);
      container
        .Building<IDisposable>()
        .Building<MemoryStream>()
        .Treat<byte[]>()
        .AsInstance(new byte[1]);

      foreach (var type in _types)
        container
          .Building(type)
          .Treat<IDisposable>()
          .AsCreated<MemoryStream>()
          .UsingParameters(0);

      
      var mem1 = dotMemory.Check();
      
      var sw = new Stopwatch();
      sw.Start();

      for (var i = 0; i < 100_000; i++)
      {
        var obj = container.Build<IDisposable>();
        Assert.AreSame(obj, expected);
      }

      sw.Stop();
      
      dotMemory.Check(memory => Console.WriteLine(memory.GetTrafficFrom(mem1)));
      dotMemoryApi.SaveCollectedData();

      
      Console.WriteLine(sw.Elapsed);
    }

    [Test, Timeout(500)]
    public void AddOrGetUnitTestMatcherTest()
    {
      const int count = 50_000;

      var builder = new Builder("stage");
      for (var i = 0; i < count; i++)
      {
        var u1 = new UnitInfo(i, null);
        var u2 = new UnitInfo(i, i);
        var str = i.ToString();
        var u3 = new UnitInfo(str, null);
        var u4 = new UnitInfo(str, str);

        builder.Treat(u1).AsCreatedWith(() => null);
        builder.Treat(u2).AsInstance(null);
        builder.Treat(u3).AsCreatedWith(() => null);
        builder.Treat(u4).AsCreatedWith(() => null).AsSingleton();
      }
    }
    
    private static Builder CreateTarget(Builder parent = null)
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            }),

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              CreateParameterValueForInjectPointBuildAction.Instance,
              CreateParameterValueBuildAction.Instance
            }),

        new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new CreatePropertyValueBuildAction()
            })
      };

      var buildStages = new object[] {BuildStage.Cache, BuildStage.Initialize, BuildStage.Create};
      var builder = parent == null ? new Builder(buildStages) : new Builder(buildStages, parent);
      builder.Children.Add(treatAll);
      return builder;
    }
  }
}