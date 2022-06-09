using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public delegate IBuildChainPattern AddContextPatterns(IBuildChainPattern node);

public abstract class TunerBase : IInternal<IBuildChainPattern, IBuildChainPattern, AddContextPatterns?>
{
  protected readonly IBuildChainPattern TreeRoot;
  protected readonly IBuildChainPattern TunedNode;

  protected readonly AddContextPatterns? ContextFactory;

  protected int Weight;

  protected TunerBase(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns? contextFactory)
  {
    TreeRoot       = treeRoot  ?? throw new ArgumentNullException(nameof(treeRoot));
    TunedNode      = tunedNode ?? throw new ArgumentNullException(nameof(tunedNode));
    ContextFactory = contextFactory;
  }

  IBuildChainPattern IInternal<IBuildChainPattern>.                                          Member1 => TunedNode;
  IBuildChainPattern IInternal<IBuildChainPattern, IBuildChainPattern>.                      Member2 => TreeRoot;
  AddContextPatterns? IInternal<IBuildChainPattern, IBuildChainPattern, AddContextPatterns?>.Member3 => ContextFactory;
}