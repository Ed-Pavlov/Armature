using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  public record SimpleToStringImpl
  {
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
