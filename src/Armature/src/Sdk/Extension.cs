using Armature.Core;

namespace Armature.Sdk;

public static class Extension
{
  public static IBuildChainPattern TryAddContext(this IBuildChainPattern node, ITuner leafTuner)
  {
    var tuner = leafTuner;

    while(tuner != null)
    {
      node   = tuner.GetOrAddNodeTo(node);
      tuner = tuner.Parent;
    }

    return node;
  }

  public static IBuildChainPattern BuildBranch(this ITuner self)
  {
    var tuner = self;
    var node  = tuner.TreeRoot;

    do
    {
      node  = tuner.GetOrAddNodeTo(node);
      tuner = tuner.Parent;
    } while(tuner != null);

    return node;
  }
}
