using System;
using System.Threading;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using JetBrains.Lifetimes;

namespace Tests.Extensibility.LifetimeRegistration;

public static class RegistrationContextExtension
{
  public static ISettingTuner AsSingleton(this ISettingTuner tuner, Lifetime lifetime)
  {
    if(tuner == null) throw new ArgumentNullException(nameof(tuner));

    tuner.AsSingleton(); // register as singleton

    // register one lifetime for all dependencies which is built in context of this 'singleton'
    // register one lifetime for singleton itself, it is needed because 'this' singleton could be built as a dependency for another singleton
    // which registered its lifetime for all its dependencies. but 'this' lifetime should be used for 'this' singleton, not 'parent' one
    // this build plan will have greater weight
    tuner.UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {tuner}")));

    // buildStackPattern.UseBuildAction(
    //     new CreateWithFactoryMethod<Lifetime>(_ => lifetime.CreateSubLifetime($"Lifetime.Of: {buildStackPattern}")),
    //     BuildStage.Create);

    // treeRoot
    //    .GetOrAddNode(new IfFirstUnit(new IsMethodParameterWithType(new UnitPattern(typeof(Lifetime), null)), WeightOf.InjectionPoint.ByExactType + WeightOf.BuildStackPattern.TargetUnit))
    //    .TryAddContext(addContextPatterns)
    //    .TreatAll()
    //    .UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Of: {buildStackPattern}")));
    //
    // tuner.UsingArguments(ForParameter.OfType<Lifetime>().UseFactoryMethod(() => lifetime.CreateSubLifetime($"Lifetime.Building: {buildStackPattern}")));

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
  public static ISettingTuner AsSingleton2(this ISettingTuner settingTuner, Lifetime lifetime)
  {
    if(settingTuner == null) throw new ArgumentNullException(nameof(settingTuner));

    var singletonLifetime = lifetime.CreateSubLifetime($"Singleton${settingTuner}");
    settingTuner
     .UsingArguments(ForParameter.OfType<Lifetime>().UseValue(singletonLifetime))
     .AsSingleton2();

    settingTuner
     .BuildingIt()
     .TreatAll()
     .UsingArguments(ForParameter.OfType<Lifetime>().UseValue(singletonLifetime));

    return settingTuner;
  }

  /// <summary>
  /// The alternative implementation of <see cref="ISettingTuner.AsSingleton"/> which is thread-safe (i.e. supports concurrent creation requests).
  /// </summary>
  public static ISettingTuner AsSingleton2(this ISettingTuner settingTuner)
  {
    if(settingTuner == null) throw new ArgumentNullException(nameof(settingTuner));

    var tuner = (ITuner) settingTuner;
    tuner.Apply().UseBuildAction(new ThreadSafeSingletonBuildAction(), BuildStage.Cache);
    return settingTuner;
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