using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Lifetimes;

namespace Tests.Extensibility.LifetimeRegistration;

public static class RegistrationContextExtension
{
  public static IFinalAndContextTuner AsSingleton([NotNull] this IFinalAndContextTuner tuner, Lifetime lifetime)
  {
    if(tuner == null) throw new ArgumentNullException(nameof(tuner));

    tuner.AsSingleton(); // register as singleton

    // register one lifetime for all dependencies which is built in context of this 'singleton'
    // register one lifetime for singleton itself, it is needed because 'this' singleton could be built as a dependency for another singleton
    // which registered its lifetime for all its dependencies. but 'this' lifetime should be used for 'this' singleton, not 'parent' one
    // this build plan will have greater weight
    tuner.UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {tuner}")));

    // buildChainPattern.UseBuildAction(
    //     new CreateWithFactoryMethod<Lifetime>(_ => lifetime.CreateSubLifetime($"Lifetime.Of: {buildChainPattern}")),
    //     BuildStage.Create);

    // treeRoot
    //    .GetOrAddNode(new IfFirstUnit(new IsMethodParameterWithType(new UnitPattern(typeof(Lifetime), null)), WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
    //    .TryAddContext(addContextPatterns)
    //    .TreatAll()
    //    .UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {buildChainPattern}")));
    //
    // tuner.UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Building: {buildChainPattern}")));

    tuner
     .BuildingIt()
     .TreatAll()
     .UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Building: {tuner}")));

    return tuner;
  }

  /// <summary>
  /// The alternative implementation of <see cref="AsSingleton"/>. It is thread-safe and reserves singleton's sub-lifetime at registration time (instead of instantiation one).
  /// </summary>
  /// <remarks>
  /// Notes about termination order:<br/>
  /// * all non-singleton dependencies of registered singleton will be terminated AFTER this singleton termination;<br/>
  /// * all singleton dependencies of registered singleton will be terminated IN REVERSE ORDER OF ITS REGISTRATION.<br/>
  /// <br/>
  /// See `ArmatureAsSingleton2Test` for termination order examples.<br/>
  /// <br/>
  /// For issues related to original implementation <see cref="AsSingleton"/> see `ArmatureSingletonLifetimeDemo` test fixture.<br/>
  /// <br/>
  /// IMPORTANT! This is experimental non-well tested stuff, use it only if you absolutely sure what you do.
  /// </remarks>
  public static IFinalAndContextTuner AsSingleton2([NotNull] this IFinalAndContextTuner finalTuner, Lifetime lifetime)
  {
    if(finalTuner == null) throw new ArgumentNullException(nameof(finalTuner));

    var singletonLifetime = lifetime.CreateSubLifetime($"Singleton${finalTuner}");
    finalTuner
     .UsingArguments(ForParameter.OfType<Lifetime>().UseValue(singletonLifetime))
     .AsSingleton2();

    finalTuner
     .BuildingIt()
     .TreatAll()
     .UsingArguments(ForParameter.OfType<Lifetime>().UseValue(singletonLifetime));

    return finalTuner;
  }

  /// <summary>
  /// The alternative implementation of <see cref="IFinalAndContextTuner.AsSingleton"/> which is thread-safe (i.e. supports concurrent creation requests).
  /// </summary>
  public static IFinalAndContextTuner AsSingleton2([NotNull] this IFinalAndContextTuner finalTuner)
  {
    if(finalTuner == null) throw new ArgumentNullException(nameof(finalTuner));

    var tuner = (ITunerInternal) finalTuner;
    tuner.BuildBranch().UseBuildAction(new ThreadSafeSingletonBuildAction(), BuildStage.Cache);
    return finalTuner;
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