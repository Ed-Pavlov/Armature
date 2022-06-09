using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Lifetimes;

namespace Tests.Extensibility.LifetimeRegistration;

public static class RegistrationContextExtension
{
  public static FinalTuner AsSingleton([NotNull] this FinalTuner tuner, Lifetime lifetime)
  {
    if(tuner == null) throw new ArgumentNullException(nameof(tuner));

    tuner.AsSingleton(); // register as singleton

    var buildChainPattern = tuner.GetInternals().Member4;
    var treeRoot          = tuner.GetInternals().Member1;
    var unitPattern       = tuner.GetInternals().Member3!;

    // register one lifetime for singleton itself, it is needed because 'this' singleton could be built as a dependency for another singleton
    // which registered its lifetime for all its dependencies. but 'this' lifetime should be used for 'this' singleton, not 'parent' one
    // this build plan will have greater weight
    tuner.UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {buildChainPattern}")));

    // register lifetime for all dependencies which is built in context of this 'singleton'
    // Building(unitPattern).TreatAll().UsingArguments(...)
    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new SkipTillUnit(unitPattern, WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.SkipTillUnit));

    new DependencyTuner(treeRoot, AddContextTo, unitPattern, treeRoot)
       .UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {buildChainPattern}")));

    return tuner;
  }

  /// <summary>
  /// The alternative implementation of <see cref="FinalTuner.AsSingleton"/> which is thread-safe (i.e. supports concurrent creation requests).
  /// </summary>
  public static FinalTuner AsSingleton2([NotNull] this FinalTuner tuner)
  {
    if(tuner == null) throw new ArgumentNullException(nameof(tuner));

    var matcher = tuner.GetInternals().Member1;
    matcher.UseBuildAction(new ThreadSafeSingletonBuildAction(), BuildStage.Cache);
    return tuner;
  }

  private sealed class ThreadSafeSingletonBuildAction : IBuildAction
  {
    private const int TimeoutMs = 800;

    private const int NoInstance       = 0;
    private const int BuildingInstance = 1;
    private const int HasInstance      = 2;

    private int    myInstanceFlag;
    private object myInstance;

    public void Process(IBuildSession buildSession)
    {
      while(true)
      {
        if(Volatile.Read(ref myInstanceFlag) == HasInstance)
        {
          buildSession.BuildResult = new BuildResult(Volatile.Read(ref myInstance));
          return;
        }

        if(Interlocked.CompareExchange(ref myInstanceFlag, BuildingInstance, NoInstance) == NoInstance)
          return;

        var spinWait  = new SpinWait();
        var startTime = Environment.TickCount;

        while(Volatile.Read(ref myInstanceFlag) == BuildingInstance)
        {
          spinWait.SpinOnce();

          if(Environment.TickCount - startTime > TimeoutMs)
            throw new TimeoutException("Waiting for singleton is timed out.");
        }
      }
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
      {
        if(Volatile.Read(ref myInstanceFlag) == HasInstance)
          return;

        Volatile.Write(ref myInstance, buildSession.BuildResult.Value);

        var ret = Interlocked.CompareExchange(ref myInstanceFlag, HasInstance, BuildingInstance);

        if(ret != BuildingInstance)
          throw new InvalidOperationException($"Internal inconsistency: unable to switch to `HasInstance`, unexpected flag {ret}");
      }
      else
      {
        var ret = Interlocked.CompareExchange(ref myInstanceFlag, NoInstance, BuildingInstance);

        if(ret != BuildingInstance)
          throw new InvalidOperationException($"Internal inconsistency: unable to switch to `NoInstance`, unexpected flag {ret}");
      }
    }
  }
}