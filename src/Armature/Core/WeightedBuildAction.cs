using System;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// <see cref="IBuildAction"/> coupled with matching weight
  /// </summary>
  public struct WeightedBuildAction
  {
    public readonly int Weight;
    public readonly IBuildAction BuildAction;

    public WeightedBuildAction(int weight, [NotNull] IBuildAction buildAction)
    {
      if (buildAction == null) throw new ArgumentNullException("buildAction");
      Weight = weight;
      BuildAction = buildAction;
    }

    public override string ToString()
    {
      return string.Format("{0}, Weight={1}", BuildAction, Weight);
    }
  }
}