using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions
{
  /// <summary>
  ///   This container is used mostly for "default" build actions applied to any unit under construction.
  ///   For example by default we want to find attributed constructor and if there is no any get longest constructor, set these two actions in right order
  ///   into <see cref="OrderedBuildActionContainer" /> to reach such behaviour.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new OrderedBuildActionContainer
  ///   {
  ///   new GetInjectPointConstructorBuildAction(),
  ///   new GetLongestConstructorBuildAction()
  ///   }
  /// </remarks>
  public class OrderedBuildActionContainer : IBuildAction, IEnumerable
  {
    private readonly List<IBuildAction> _buildActions = new List<IBuildAction>();
    private readonly ConcurrentDictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new ConcurrentDictionary<IBuildSession, IBuildAction>();

    public void Process(IBuildSession buildSession)
    {
      var exceptions = new List<Exception>();
      foreach (var buildAction in _buildActions)
      {
        try
        {
          buildAction.Process(buildSession);

          if (!buildSession.BuildResult.HasValue)
            Log.WriteLine(LogLevel.Trace, () => string.Format("{0} has not built value", buildAction));
          else
          {
            Log.WriteLine(LogLevel.Info, () => string.Format("redirected execution to {0}", buildAction));
            _effectiveBuildActions.TryAdd(buildSession, buildAction);
            break;
          }
        }
        catch (ArmatureException exc)
        {
          LogException(exc);
          exceptions.Add(exc);
          // continue;
        }
        catch (Exception exc)
        {
          Log.WriteLine(LogLevel.Trace, () => string.Format("User exception was throw during executing {0}", buildAction));
          LogException(exc);
          for (var i = 0; i < exceptions.Count; i++)
            exc.AddData(i, exceptions[i]);
          throw;
        }
      }

      if (!buildSession.BuildResult.HasValue && exceptions.Count > 0)
      {
        var exception = new ArmatureException("Multiply exceptions occured during processing build actions");
        for (var i = 0; i < exceptions.Count; i++)
          exception.AddData(i, exceptions[i]);
        throw exception;
      }
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if (_effectiveBuildActions.TryRemove(buildSession, out var effectiveBuildAction))
      {
        effectiveBuildAction.PostProcess(buildSession);
        Log.WriteLine(LogLevel.Info, () => string.Format("redirected execution to {0}", effectiveBuildAction));
      }
    }

    public IEnumerator GetEnumerator() => throw new NotSupportedException();

    public OrderedBuildActionContainer Add(IBuildAction buildAction)
    {
      _buildActions.Add(buildAction);
      return this;
    }

    private static void LogException(Exception exc) =>
      LogLevel.Trace.ExecuteIfEnabled(
        () =>
          {
            using (Log.Block(LogLevel.Trace, "Exception: {0}", exc))
            {
              foreach (DictionaryEntry entry in exc.Data)
                Log.WriteLine(LogLevel.Trace, "{0}: {1}", entry.Key, entry.Value);
            }
          });

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}