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
    private readonly List<UnitId>          _buildSequence;
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
      _buildSequence  = new List<UnitId>(4);
      _parentBuilders = parentBuilders;
    }

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    /// <param name="buildStages">The conveyor of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static BuildResult BuildUnit(
      UnitId                unitId,
      IEnumerable<object>   buildStages,
      BuildPlansCollection  buildPlans,
      BuildPlansCollection? runtimeBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildUnit(unitId);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    /// <param name="buildStages">The conveyor of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static IReadOnlyList<BuildResult>? BuildAllUnits(
      UnitId                unitId,
      IEnumerable<object>   buildStages,
      BuildPlansCollection  buildPlans,
      BuildPlansCollection? runtimeBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildAllUnits(unitId);

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public BuildResult BuildUnit(UnitId unitId) => Build(unitId, BuildUnit);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public IReadOnlyList<BuildResult>? BuildAllUnits(UnitId unitId) => Build(unitId, BuildAllUnits);

    private T? Build<T>(UnitId unitId, Func<BuildActionBag?, T> build)
    {
      using(LogBuildSessionState(unitId))
      {
        _buildSequence.Add(unitId);

        try
        {
          BuildActionBag? actions;
          BuildActionBag? auxActions;

          using(Log.Block(LogLevel.Verbose, "Looking for build actions"))
          {
            actions    = _buildPlans.GatherBuildActions(_buildSequence.AsArrayTail());
            auxActions = _auxBuildPlans?.GatherBuildActions(_buildSequence.AsArrayTail());
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
    private BuildResult BuildUnit(BuildActionBag? BuildActionBag)
    {
      if(BuildActionBag is null)
        return BuildViaParentBuilder(_buildSequence.Last());

      // builder to pass into IBuildActon.Execute
      var unitBuilder      = new Interface(_buildSequence, this);
      var performedActions = new Stack<IBuildAction>();

      foreach(var stage in _buildStages)
      {
        var buildAction = BuildActionBag.GetTopmostAction(stage);

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

    private BuildResult BuildViaParentBuilder(UnitId unitId)
    {
      if(_parentBuilders is null) return default;

      var exceptions = new List<Exception>();

      for(var i = 0; i < _parentBuilders.Length; i++)
        try
        {
          using(Log.Block(LogLevel.Info, "Try build via parent builder #{0}", i))
          {
            var buildResult = _parentBuilders[i].BuildUnit(unitId, _auxBuildPlans);

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
    private List<BuildResult>? BuildAllUnits(BuildActionBag? BuildActionBag)
    {
      if(BuildActionBag is null) return null;

      if(BuildActionBag.Keys.Count > 1)
        throw new ArmatureException("Actions only for one stage should be provided for BuildAll");

      var result = new List<BuildResult>();

      foreach(var buildAction in BuildActionBag.Values.Single().Select(_ => _.Entity))
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

    private IDisposable LogBuildSessionState(UnitId unitId)
    {
      var block = Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Build", unitId));

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
