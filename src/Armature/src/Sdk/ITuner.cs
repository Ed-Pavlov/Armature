using Armature.Core;

namespace Armature.Sdk;

public interface ITuner
{
  ITuner?            Parent   { get; }
  IBuildChainPattern TreeRoot { get; }
  int                Weight   { get; }
  IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node);
}
