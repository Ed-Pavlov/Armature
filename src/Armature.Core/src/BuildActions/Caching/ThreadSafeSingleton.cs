using System;
using System.Diagnostics;
using System.Threading;
using Armature.Core.Sdk;

namespace Armature.Core;

public record ThreadSafeSingleton : IBuildAction, ILogString
{
  private const int NoInstance       = 0;
  private const int BuildingUnit = 1;
  private const int HasInstance      = 2;
  private int _state = NoInstance;

  private readonly int _timeout;
  private object? _instance;

  public ThreadSafeSingleton() : this(500) { }
  public ThreadSafeSingleton(int timeout) => _timeout = timeout;

  public void Process(IBuildSession buildSession)
  {
    while(true)
    {
      if(Volatile.Read(ref _state) == HasInstance)
      {
        buildSession.BuildResult = new BuildResult(Volatile.Read(ref _instance));
        return;
      }

      if(Interlocked.CompareExchange(ref _state, BuildingUnit, NoInstance) == NoInstance)
        return;

      // the instance is building in another build session on another thread, wait for it
      var spinWait  = new SpinWait();
      var startTime = Environment.TickCount;

      while(Volatile.Read(ref _state) == BuildingUnit)
      {
        spinWait.SpinOnce();

        if(Environment.TickCount - startTime > _timeout)
          throw new ArmatureException("Timeout has expired while waiting for the value being built on another thread");
      }
    }
  }

  public void PostProcess(IBuildSession buildSession)
  {
    int prevState;

    if(!buildSession.BuildResult.HasValue)
      prevState = Interlocked.CompareExchange(ref _state, NoInstance, BuildingUnit);
    else
    {
      prevState = Interlocked.CompareExchange(ref _state, HasInstance, BuildingUnit);
      switch(prevState)
      {
        case HasInstance:
          break; // already built on another thread
        case BuildingUnit: Volatile.Write(ref _instance, buildSession.BuildResult.Value);
          break;
      }
    }

    // paranoia mode on
    if(prevState != BuildingUnit)
      throw new InvalidOperationException($"{LogConst.BuildAction_PostProcess(this)} is called while the build action not in the state {nameof(BuildingUnit)}");
  }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(Singleton)}{{ Instance: {(Thread.VolatileRead(ref _state) == HasInstance ? _instance.ToHoconString() : "nothing")} }} }}";

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}