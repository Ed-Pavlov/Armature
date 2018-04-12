using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Logging;

namespace Armature.Core.BuildActions
{
  /// <summary>
  /// This container is used mostly for "default" build actions applyed to any unit under construction.
  /// For example by default we want to find attributed constructor and if there is no any get longest constructor, set these two actions in right order
  /// into <see cref="OrderedBuildActionContainer"/> to reach such behaviour.
  /// </summary>
  /// <remarks>This class implements <see cref="IEnumerable"/> and has <see cref="Add"/> method in order to make possible compact and readable initialization like
  /// new OrderedBuildActionContainer
  /// {
  ///   new GetInjectPointConstructorBuildAction(),
  ///   new GetLongesConstructorBuildAction()
  /// }
  /// </remarks>
  public class OrderedBuildActionContainer : IBuildAction, IEnumerable
  {
    private readonly List<IBuildAction> _buildActions = new List<IBuildAction>();
    private IBuildAction _effectiveBuildAction;
    
    public OrderedBuildActionContainer Add(IBuildAction buildAction)
    {
      _buildActions.Add(buildAction);
      return this;
    }

    public void Process(IBuildSession buildSession)
    {
      foreach (var buildAction in _buildActions)
      {
        try
        {
          buildAction.Process(buildSession);
          if (buildSession.BuildResult != null)
          {
            Log.WriteLine(LogLevel.Info, "redirected execution to {0}", buildAction);
            _effectiveBuildAction = buildAction;
            break;
          }
        }
        catch (ArmatureException exc)
        {
          LogException(exc);
          // ReSharper disable once RedundantJumpStatement
          continue;
        }
        catch (Exception exc)
        {
          Log.WriteLine(LogLevel.Trace, "User exception was throw during executing {0}", buildAction);
          LogException(exc);
          throw;
        }
      }
    }

    private static void LogException(Exception exc)
    {
      using (Log.Block(LogLevel.Trace, "Exception: {0}", exc))
        foreach (DictionaryEntry entry in exc.Data)
          Log.WriteLine(LogLevel.Trace, "{0}: {1}", entry.Key, entry.Value);
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if (_effectiveBuildAction != null)
      {
        _effectiveBuildAction.PostProcess(buildSession);
        Log.WriteLine(LogLevel.Info, "redirected execution to {0}", _effectiveBuildAction);
        _effectiveBuildAction = null;
      }
    }

    public IEnumerator GetEnumerator() => throw new NotSupportedException();
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}