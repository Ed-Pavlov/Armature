using Armature.Core;

namespace Armature.Sdk;

public interface ITuner
{
  ITuner?    Parent   { get; }
  IBuildChainPattern TreeRoot { get; }
  IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node);
  int                Weight { get; }
}
