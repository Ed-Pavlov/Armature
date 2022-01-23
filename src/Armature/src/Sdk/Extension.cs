using Armature.Core;

namespace Armature.Sdk;

public static class Extension
{
  public static IBuildChainPattern TryAddContext(this IBuildChainPattern node, AddContextPatterns? getContext)
    => getContext is null ? node : getContext(node);
}