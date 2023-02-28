using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// This container is used mostly for "default" build actions applied to any target unit.
/// For example by default we want to find attributed constructor and if there is no any get the constructor with the largest number of parameters,
/// set these two actions in right order into <see cref="TryInOrder" /> to reach such behaviour. If a build action did not build a unit, container
/// calls the next one till a unit will be built.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
/// new OrderedBuildActionContainer
/// {
///  new GetConstructorByInjectPoint (),
///  new GetConstructorWithMaxParametersCount ()
/// }
/// </remarks>
public record TryInOrder : IBuildAction, IEnumerable, ILogString
{
  private readonly List<IBuildAction>                      _buildActions;
  private readonly Dictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

  public TryInOrder() => _buildActions = new List<IBuildAction>();
  public TryInOrder(params IBuildAction[] buildActions)
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
        using(var condition = Log.UnderCondition(LogLevel.Verbose))
        using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_Process(buildAction)))
        {
          buildAction.Process(buildSession);
          condition.IsMet = buildSession.BuildResult.HasValue;
        }

        if(buildSession.BuildResult.HasValue)
        {
          _effectiveBuildActions.Add(buildSession, buildAction);
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
    if(_effectiveBuildActions.TryGetValue(buildSession, out var buildAction))
    {
      _effectiveBuildActions.Remove(buildSession);

      using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_PostProcess(buildAction)))
        buildAction.PostProcess(buildSession);
    }
  }

  public virtual bool Equals(TryInOrder? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;
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

  public TryInOrder Add(IBuildAction buildAction)
  {
    if(buildAction is null) throw new ArgumentNullException(nameof(buildAction));

    _buildActions.Add(buildAction);
    return this;
  }

  [WithoutTest]
  public IEnumerator GetEnumerator() => throw new NotSupportedException();

  [DebuggerStepThrough]
  public override string ToString() => GetType().ToLogString();

  public string ToHoconString() => $"{{ {nameof(TryInOrder)} {{ Actions: {_buildActions.ToHoconString()} }} }}";
}
