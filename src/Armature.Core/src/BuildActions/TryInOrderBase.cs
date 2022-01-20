using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

public abstract class TryInOrderBase : IBuildAction, ILogString, IEnumerable
{
  private readonly List<IBuildAction> _buildActions;

  protected TryInOrderBase() => _buildActions = new List<IBuildAction>();
  protected TryInOrderBase(params IBuildAction[] buildActions)
  {
    if(buildActions is null) throw new ArgumentNullException(nameof(buildActions));
    if(buildActions.Any(_ => _ is null)) throw new ArgumentNullException(nameof(buildActions), "Items should not be null");

    _buildActions = buildActions.ToList();
  }

  public void Process(IBuildSession buildSession)
  {
    var exceptions = new List<Exception>();

    foreach(var buildAction in _buildActions)
      try
      {
        using(Log.ConditionalMode(LogLevel.Verbose, () => buildSession.BuildResult.HasValue))
        using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_Process(buildAction)))
          buildAction.Process(buildSession);

        if(buildSession.BuildResult.HasValue)
        {
          StoreBuildAction(buildSession, buildAction);
          break;
        }
      }
      catch(ArmatureException exception)
      { // ArmatureException is a valid way to indicate that build action can't build a unit, gather such exceptions to report them all
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_Process(buildAction)}.Exception: "))
          exception.WriteToLog();

        exceptions.Add(exception);
      }
      catch(Exception exception)
      { // User exception is another matter
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_Process(buildAction)}.Exception: "))
          exception.WriteToLog();

        throw; // don't try to call other actions, they can return "wrong" unit, user exception is unexpected case
      }

    if(!buildSession.BuildResult.HasValue)
      if(exceptions.Count == 1)
        ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
      else if(exceptions.Count > 0)
        throw new ArmatureException(
                $"{exceptions.Count} exceptions from executed build actions. "
              + $"See {nameof(Exception)}.{nameof(Exception.Data)} and {nameof(ArmatureException)}.{nameof(ArmatureException.InnerExceptions)}"
              + $" for details or enable logging using {nameof(Log)}.{nameof(Log.Enable)} to investigate the error.",
                exceptions)
           .AddData(ExceptionConst.Logged, true);
  }

  public void PostProcess(IBuildSession buildSession)
  {
    if(DiscardBuildAction(buildSession, out var buildAction))
    {
      using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_PostProcess(buildAction)))
        buildAction.PostProcess(buildSession);
    }
  }

  protected abstract void StoreBuildAction(IBuildSession   buildSession, IBuildAction     buildAction);
  protected abstract bool DiscardBuildAction(IBuildSession buildSession, out IBuildAction buildAction);

  public virtual bool Equals(TryInOrderBase? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;
    if(GetType() != other.GetType()) return false;

    if(_buildActions.Count != other._buildActions.Count) return false;
    for(var i = 0; i < _buildActions.Count; i++)
      if(!_buildActions[i].Equals(other._buildActions[i]))
        return false;

    return true;
  }

  public override int GetHashCode()
  {
    var hash = _buildActions.Count.GetHashCode();

    foreach(var buildAction in _buildActions)
      hash ^= buildAction.GetHashCode();

    return hash;
  }

  public void Add(IBuildAction buildAction)
  {
    if(buildAction is null) throw new ArgumentNullException(nameof(buildAction));
    _buildActions.Add(buildAction);
  }

  [WithoutTest]
  public IEnumerator GetEnumerator() => throw new NotSupportedException();

  [DebuggerStepThrough]
  public override string ToString() => GetType().ToLogString();

  public string ToHoconString() => $"{{ {nameof(TryInOrder)} {{ Actions: {_buildActions.ToHoconString()} }} }}";
}