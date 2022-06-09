using System;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    /// <summary>
    /// Specifies what unit should be built to fill <see cref="Maybe{T}" /> value.
    /// </summary>
    public static TreatingTuner<T> TreatMaybeValue<T>(this TreatingTuner<Maybe<T>> treatingTuner)
    {
      var tuner          = treatingTuner.GetInternals();
      var treeRoot       = tuner.Member1;
      var contextFactory = tuner.Member2!;
      var tunedNode      = tuner.Member4;

      var uniqueTag = Guid.NewGuid();

      tunedNode.UseBuildAction(new BuildMaybeAction<T>(uniqueTag), BuildStage.Create);

      var unitPattern = new UnitPattern(typeof(T), uniqueTag);

      var valueNode = treeRoot
                     .GetOrAddNode(new IfFirstUnit(unitPattern, 0)) //TODO: weight
                     .TryAddContext(contextFactory);

      IBuildChainPattern AddContextTo(IBuildChainPattern node) => node.GetOrAddNode(new IfFirstUnit(unitPattern, 0)).TryAddContext(contextFactory);

      return new TreatingTuner<T>(treeRoot, AddContextTo, unitPattern, valueNode);
    }

    /// <summary>
    /// Specifies that value from built <see cref="Maybe{T}" /> should be used as a unit.
    /// </summary>
    public static TreatingTuner<Maybe<T>> AsMaybeValueOf<T>(this TreatingTuner<T> treatingTuner)
    {
      var tuner             = treatingTuner.GetInternals();
      var treeRoot          = tuner.Member1;
      var contextFactory    = tuner.Member2!;
      var buildChainPattern = tuner.Member4;

      return new TreatingTuner<Maybe<T>>(
          treeRoot,
          contextFactory,
          null!,
          buildChainPattern.UseBuildAction(new GetMaybeValueBuildAction<T>(), BuildStage.Initialize));
    }
  }
}