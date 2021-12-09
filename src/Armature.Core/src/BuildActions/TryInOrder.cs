using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// This container is used mostly for "default" build actions applied to any unit under construction.
/// For example by default we want to find attributed constructor and if there is no any get longest constructor, set these two actions in right order
/// into <see cref="TryInOrder" /> to reach such behaviour. If a build action did not build a unit container calls the next one till
/// a unit will be built.
/// </summary>
/// <remarks>
/// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
/// new OrderedBuildActionContainer
/// {
///  new GetInjectPointConstructorBuildAction(),
///  new GetLongestConstructorBuildAction()
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

    Log.WriteLine(LogLevel.Verbose, "");
    foreach(var buildAction in _buildActions)
      try
      {
        using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_Process(buildAction)))
          buildAction.Process(buildSession);

        if(buildSession.BuildResult.HasValue)
        {
          _effectiveBuildActions.Add(buildSession, buildAction);
          break;
        }
      }
      catch(Exception exc)
      {
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_Process(buildAction)}.Exception: "))
          exc.WriteToLog();
        exceptions.Add(exc);
      }

    if(!buildSession.BuildResult.HasValue)
      if(exceptions.Count > 0)
        throw new AggregateException(
            $"{exceptions.Count} exceptions occured during executing build actions. "
          + $"See {nameof(Exception)}.{nameof(Exception.Data)} and {nameof(AggregateException)}.{nameof(AggregateException.InnerExceptions)}"
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