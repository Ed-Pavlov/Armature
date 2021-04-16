using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class implementing the logic of adding build actions
  /// </summary>
  /// <remarks>
  /// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  /// new FooQuery
  /// {
  ///   new SubQuery(ConstructorMatcher.Instance, 0)
  ///     .UseBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///   new SubQuery(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
  ///     .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  /// };
  /// </remarks>
  public abstract class Query : IQuery, IEnumerable
  {
    private Dictionary<object, List<IBuildAction>>? _buildActions;

    protected Query(int weight) => Weight = weight;

    protected int Weight { get; }

    private Dictionary<object, List<IBuildAction>> LazyBuildAction
    {
      [DebuggerStepThrough] get => _buildActions ??= new Dictionary<object, List<IBuildAction>>();
    }

    public abstract ICollection<IQuery> Children { get; }

    public abstract BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);

    [DebuggerStepThrough]
    public virtual IQuery UseBuildAction(object buildStage, IBuildAction buildAction)
    {
      LazyBuildAction
       .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
       .Add(buildAction);

      return this;
    }

    public abstract bool Equals(IQuery other);

    [DebuggerStepThrough]
    protected BuildActionBag? GetOwnActions(int matchingWeight)
    {
      if(_buildActions is null) return null;

      var result = new BuildActionBag();

      foreach(var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>", GetType().GetShortName(), Weight);

    public void PrintToLog()
    {
      ICollection<IQuery>? children = null;

      try
      {
        children = Children;
      }
      catch
      { // do nothing
      }

      if(children is not null)
        foreach(var child in children)
          using(Log.Block(LogLevel.Info, child.ToString()))
          {
            child.PrintToLog();
          }

      if(_buildActions is not null)
        using(Log.Block(LogLevel.Info, "Build actions"))
        {
          foreach(var pair in _buildActions)
            using(Log.Block(LogLevel.Info, "Stage: {0}", pair.Key))
            {
              foreach(var buildAction in pair.Value)
                if(buildAction is ILogable printable)
                  printable.PrintToLog();
                else
                  Log.Info(buildAction.ToString());
            }
        }
    }

    #region Syntax sugar

    public void             Add(IQuery query) => Children.Add(query);
    IEnumerator IEnumerable.GetEnumerator()   => throw new NotSupportedException();

    #endregion
  }
}
