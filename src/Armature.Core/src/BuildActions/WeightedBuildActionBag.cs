using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Armature.Core.Internal;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Collection of build actions with weight grouped by a build stage.
/// </summary>
public class WeightedBuildActionBag : Dictionary<object, LeanList<Weighted<IBuildAction>>>
{
  [DebuggerStepThrough]
  public override string ToString()
  {
    var sb = new StringBuilder(string.Format("{0} matched stages\n", Count));

    foreach(var pair in this)
    {
      sb.AppendFormat("\t{0}: ", pair.Key);

      foreach(var action in pair.Value)
      {
        sb.AppendFormat("\t{0}:{1}", action.Weight, action.Entity);
        sb.AppendLine();
      }
    }

    return sb.ToString();
  }
}