using System;
using Armature.Core.Annotations;

namespace Armature.Core;

/// <summary>
///   Skips all units in the build chain and pass the last (under construction) unit  to <see cref="IBuildChainPattern.Children" />.
/// </summary>
public class SkipAllUnits : BuildChainPatternWithChildrenBase
{
  public SkipAllUnits() : this(WeightOf.BuildContextPattern.SkipAllUnits){}
  public SkipAllUnits(int weight) : base(weight) { }

  [WithoutTest]
  public override BuildActionBag BuildActions
    => throw new NotSupportedException(
         "This pattern is used to skip a build chain to the unit under construction (the last one) and pass it to children."
       + "It can't contain build actions due to they are used to build the unit under construction only."
       );

  /// <summary>
  ///   Decreases the matching weight by each skipped unit then pass unit under construction to children nodes
  /// </summary>
  public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> buildChain, int inputWeight)
  {
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipAllUnits)))
    {
      Log.WriteLine(LogLevel.Verbose, $"Weight = {Weight}");
      var lastUnitAsTail = buildChain.GetTail(buildChain.Length - 1);
      return GetChildrenActions(lastUnitAsTail, inputWeight + Weight);
    }
  }
}