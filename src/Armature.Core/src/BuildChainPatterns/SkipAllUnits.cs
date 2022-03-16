using System;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Skips all units in the build chain and pass the target unit  to <see cref="IBuildChainPattern.Children" />.
/// </summary>
public class SkipAllUnits : BuildChainPatternWithChildrenBase
{
  public SkipAllUnits() : this(WeightOf.BuildChainPattern.SkipAllUnits) { }
  public SkipAllUnits(int weight) : base(weight) { }

  [WithoutTest]
  public override BuildActionBag BuildActions
    => throw new NotSupportedException(
         "This pattern is used to skip a build context right to the target unit and pass it to children."
       + "It can't contain build actions due to they are used to build the target unit."
       );

  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    var hasActions = false;

    // ReSharper disable once AccessToModifiedClosure - yes, that's the point
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipAllUnits)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Weight = {Weight.ToHoconString()}");
      var targetUnit = buildChain.GetTail(buildChain.Length - 1);

      var  decreaseWeight = (buildChain.Length - 1) * Weight;
      hasActions = GetChildrenActions(targetUnit, inputWeight + decreaseWeight, out actionBag);
      return hasActions;
    }
  }
}