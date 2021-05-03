﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   This container is used mostly for "default" build actions applied to any unit under construction.
  ///   For example by default we want to find attributed constructor and if there is no any get longest constructor, set these two actions in right order
  ///   into <see cref="OrderedBuildActionContainer" /> to reach such behaviour. If a build action did not build a unit container calls the next one till
  ///   a unit will be built.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new OrderedBuildActionContainer
  ///   {
  ///     new GetInjectPointConstructorBuildAction(),
  ///     new GetLongestConstructorBuildAction()
  ///   }
  /// </remarks>

  //TODO: need another name, something like build actions chain, or whatever, describing semantic better than now
  public class OrderedBuildActionContainer : IBuildAction, ILogable, IEnumerable
  {
    private readonly List<IBuildAction>                                _buildActions          = new();
    private readonly ConcurrentDictionary<IBuildSession, IBuildAction> _effectiveBuildActions = new();

    public void Process(IBuildSession buildSession)
    {
      var exceptions = new List<Exception>();

      foreach(var buildAction in _buildActions)
        try
        {
          using(LogBuildActionProcess(buildSession, buildAction))
          {
            buildAction.Process(buildSession);
          }

          if(buildSession.BuildResult.HasValue)
          {
            _effectiveBuildActions.TryAdd(buildSession, buildAction);
            break;
          }
        }
        catch(Exception exc)
        {
          exc.ToLog(() => $"Exception was thrown during executing {buildAction} {nameof(IBuildAction.Process)} method");
          exceptions.Add(exc);
        }

      if(!buildSession.BuildResult.HasValue && exceptions.Count > 0)
        throw exceptions.Aggregate($"{exceptions.Count} exceptions occured during processing build actions");
    }

    private static IDisposable LogBuildActionProcess(IBuildSession buildSession, IBuildAction buildAction)
      => Log.Deferred(
        LogLevel.Verbose,
        writeDeferredLog =>
        {
          var buildResult = buildSession.BuildResult;
          var logLevel    = buildResult.HasValue ? LogLevel.Verbose : LogLevel.Trace;

          string GetLogLine() => $"{buildAction}.{nameof(IBuildAction.Process)}(...) => {buildResult}";

          if(writeDeferredLog is null)
            Log.WriteLine(logLevel, GetLogLine);
          else
            using(Log.Block(logLevel, GetLogLine))
              writeDeferredLog();
        });

    public void PostProcess(IBuildSession buildSession)
    {
      if(_effectiveBuildActions.TryRemove(buildSession, out var buildAction))
      {
        buildAction.PostProcess(buildSession);

        Log.WriteLine(LogLevel.Trace, () => string.Format("{0}.{1}(...)", buildAction, nameof(IBuildAction.PostProcess)));
      }
    }

    public IEnumerator GetEnumerator() => throw new NotSupportedException();

    public OrderedBuildActionContainer Add(IBuildAction buildAction)
    {
      _buildActions.Add(buildAction);

      return this;
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().ToLogString();

    public void PrintToLog()
    {
      using(Log.Block(LogLevel.Info, nameof(OrderedBuildActionContainer)))
      {
        foreach(var buildAction in _buildActions)
          Log.WriteLine(LogLevel.Info, buildAction.ToString);
      }
    }
  }
}
