using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  public record UnitMatcherBase
  {
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
