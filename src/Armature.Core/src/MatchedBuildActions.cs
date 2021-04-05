using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Armature.Core
{
  /// <summary>
  ///   Collection of build actions from matched build plans grouped by the build stage
  /// </summary>
  public class MatchedBuildActions : Dictionary<object, List<Weighted<IBuildAction>>>
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
}
