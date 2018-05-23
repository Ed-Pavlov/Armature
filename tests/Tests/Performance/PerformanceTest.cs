using System;
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

    // ordered tree: 100 000 times, 2.21 sec
    // whole tree traverse: 100 000 times, 2.44 sec // TreeNode implementation
    // whole tree traverse: 100 000 times, 1 sec // BuildAction tree implementation

    [Test]
    [Ignore("Run manually only")]
    [AssertTraffic(AllocatedSizeInBytes = 0)]
//    [AssertTraffic(AllocatedObjectsCount = 3, Types = new[] { typeof(AssembleStage)})]
    [DotMemoryUnit(SavingStrategy = SavingStrategy.OnCheckFail)]
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
          .As<MemoryStream>()
          .UsingParameters(0);

      var sw = new Stopwatch();
      sw.Start();
      for (var i = 0; i < 100000; i++)
      {
        var obj = container.Build<IDisposable>();
        Assert.AreSame(obj, expected);
      }

      sw.Stop();
      Console.WriteLine(sw.Elapsed);
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