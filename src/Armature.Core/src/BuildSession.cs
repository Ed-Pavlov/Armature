using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Armature.Core.Common;
using Armature.Core.Logging;


namespace Armature.Core
{
  /// <summary>
  ///   Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session
  /// </summary>
  public partial class BuildSession
  {
    private readonly Builder[]?            _parentBuilders;
    private readonly BuildPlansCollection? _auxBuildPlans;
    private readonly BuildPlansCollection  _buildPlans;
    private readonly List<UnitInfo>        _buildSequence;
    private readonly IEnumerable<object>   _buildStages;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public BuildSession(
      IEnumerable<object>   buildStages,
      BuildPlansCollection  buildPlans,
      BuildPlansCollection? auxBuildPlans,
      Builder[]?            parentBuilders)
    {
      if(buildStages is null) throw new ArgumentNullException(nameof(buildStages));
      if(buildPlans is null) throw new ArgumentNullException(nameof(buildPlans));

      _buildStages    = buildStages;
      _buildPlans     = buildPlans;
      _auxBuildPlans  = auxBuildPlans;
      _buildSequence  = new List<UnitInfo>(4);
      _parentBuilders = parentBuilders;
    }

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    /// <param name="buildStages">The conveyor of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static BuildResult BuildUnit(
      UnitInfo              unitInfo,
      IEnumerable<object>   buildStages,
      BuildPlansCollection  buildPlans,
      BuildPlansCollection? runtimeBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildUnit(unitInfo);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    /// <param name="buildStages">The conveyor of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static IReadOnlyList<BuildResult>? BuildAllUnits(
      UnitInfo              unitInfo,
      IEnumerable<object>   buildStages,
      BuildPlansCollection  buildPlans,
      BuildPlansCollection? runtimeBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildAllUnits(unitInfo);

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public BuildResult BuildUnit(UnitInfo unitInfo) => Build(unitInfo, BuildUnit);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public IReadOnlyList<BuildResult>? BuildAllUnits(UnitInfo unitInfo) => Build(unitInfo, BuildAllUnits);

    private T? Build<T>(UnitInfo unitInfo, Func<MatchedBuildActions?, T> build)
    {
      if(unitInfo is null) throw new ArgumentNullException(nameof(unitInfo));

      using(LogBuildSessionState(unitInfo))
      {
        _buildSequence.Add(unitInfo);

        try
        {
          MatchedBuildActions? actions;
          MatchedBuildActions? auxActions;

          using(Log.Block(LogLevel.Verbose, "Looking for build actions"))
          {
            actions    = _buildPlans.GetBuildActions(_buildSequence.AsArrayTail());
            auxActions = _auxBuildPlans?.GetBuildActions(_buildSequence.AsArrayTail());
          }

          Log.WriteLine(LogLevel.Verbose, "");

          return build(actions.Merge(auxActions));
        }
        catch(Exception exception)
        {
          AddDebugData(exception);

          throw;
        }
        finally
        {
          _buildSequence.RemoveAt(_buildSequence.Count - 1);
        }
      }
    }

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    private BuildResult BuildUnit(MatchedBuildActions? matchedBuildActions)
    {
      if(matchedBuildActions is null)
        return BuildViaParentBuilder(_buildSequence.Last());

      // builder to pass into IBuildActon.Execute
      var unitBuilder      = new Interface(_buildSequence, this);
      var performedActions = new Stack<IBuildAction>();

      foreach(var stage in _buildStages)
      {
        var buildAction = matchedBuildActions.GetTopmostAction(stage);

        if(buildAction is null)
          continue;

        performedActions.Push(buildAction);

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Execute action", buildAction)))
        {
          buildAction.Process(unitBuilder);
        }

        if(unitBuilder.BuildResult.HasValue)
        {
          Log.WriteLine(LogLevel.Info, "");

          Log.WriteLine(
            LogLevel.Info,
            () => string.Format("Build Result{{{0}:{1}}}", unitBuilder.BuildResult, unitBuilder.BuildResult.Value?.GetType().ToLogString()));

          break; // object is built, unwind called actions in reverse orders
        }
      }

      foreach(var buildAction in performedActions)
      {
        Log.WriteLine(LogLevel.Info, "");

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Rewind action", buildAction)))
        {
          buildAction.PostProcess(unitBuilder);
        }
      }

      return unitBuilder.BuildResult.HasValue
               ? unitBuilder.BuildResult
               : BuildViaParentBuilder(_buildSequence.Last());
    }

    private BuildResult BuildViaParentBuilder(UnitInfo unitInfo)
    {
      if(_parentBuilders is null) return default;

      var exceptions = new List<Exception>();

      for(var i = 0; i < _parentBuilders.Length; i++)
        try
        {
          using(Log.Block(LogLevel.Info, "Try build via parent builder #{0}", i))
          {
            var buildResult = _parentBuilders[i].BuildUnit(unitInfo, _auxBuildPlans);

            if(buildResult.HasValue)
              return buildResult;
          }
        }
        catch(Exception exc)
        {
          exceptions.Add(exc);

          // continue
        }

      if(exceptions.Count == 0)
        return default;

      throw exceptions.Aggregate("One or more exceptions occured during build the unit");
    }

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    private List<BuildResult>? BuildAllUnits(MatchedBuildActions? matchedBuildActions)
    {
      if(matchedBuildActions is null) return null;

      if(matchedBuildActions.Keys.Count > 1)
        throw new ArmatureException("Actions only for one stage should be provided for BuildAll");

      var result = new List<BuildResult>();

      foreach(var buildAction in matchedBuildActions.Values.Single().Select(_ => _.Entity))
      {
        var unitBuilder = new Interface(_buildSequence, this);

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Execute action", buildAction)))
        {
          buildAction.Process(unitBuilder);
          buildAction.PostProcess(unitBuilder);
        }

        if(unitBuilder.BuildResult.HasValue)
        {
          Log.WriteLine(LogLevel.Info, "");

          Log.WriteLine(
            LogLevel.Info,
            () => string.Format("Build Result{{{0}:{1}}}", unitBuilder.BuildResult, unitBuilder.BuildResult.Value?.GetType().ToLogString()));

          result.Add(unitBuilder.BuildResult);
        }
      }

      return result;
    }

    private IDisposable LogBuildSessionState(UnitInfo unitInfo)
    {
      var block = Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Build", unitInfo));

      {
        _buildSequence.ToLog();
      }

      return block;
    }

    private void AddDebugData(Exception exception)
    {
      if(exception.Data.Contains(ExceptionData.BuildSequence)) return;

      var sb = new StringBuilder();

      foreach(var unitInfo in _buildSequence)
        sb.AppendLine(unitInfo.ToString());

      exception.AddData(ExceptionData.BuildSequence, sb.ToString());
    }
  }
}
