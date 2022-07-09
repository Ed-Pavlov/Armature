using Armature.Core;

namespace Armature.Sdk;

public interface ITunerInternal
{
  ITunerInternal?    Parent   { get; }
  IBuildChainPattern TreeRoot { get; }
  IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node);
  int                Weight { get; }
}
