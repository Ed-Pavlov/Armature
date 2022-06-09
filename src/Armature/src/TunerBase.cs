using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public delegate IBuildChainPattern AddContextPatterns(IBuildChainPattern node);

public abstract class TunerBase : IInternal<IBuildChainPattern, AddContextPatterns?, IUnitPattern?, IBuildChainPattern>
{
  protected readonly IBuildChainPattern  TreeRoot;
  protected readonly AddContextPatterns? ContextFactory;
  protected readonly IUnitPattern?       UnitPattern;
  protected readonly IBuildChainPattern  TunedNode;

  protected int Weight;

  protected TunerBase(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode)
  {
    TreeRoot       = treeRoot ?? throw new ArgumentNullException(nameof(treeRoot));
    TunedNode      = tunedNode ?? throw new ArgumentNullException(nameof(tunedNode));
  }
  protected TunerBase(IBuildChainPattern treeRoot, AddContextPatterns? contextFactory, IUnitPattern? unitPattern, IBuildChainPattern tunedNode)
  {
    TreeRoot       = treeRoot ?? throw new ArgumentNullException(nameof(treeRoot));
    ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    UnitPattern    = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));
    TunedNode      = tunedNode ?? throw new ArgumentNullException(nameof(tunedNode));
  }

  IBuildChainPattern IInternal<IBuildChainPattern>.                                                        Member1 => TreeRoot;
  AddContextPatterns? IInternal<IBuildChainPattern, AddContextPatterns?>.                                  Member2 => ContextFactory;
  IUnitPattern? IInternal<IBuildChainPattern, AddContextPatterns?, IUnitPattern?>.                         Member3 => UnitPattern;
  IBuildChainPattern IInternal<IBuildChainPattern, AddContextPatterns?, IUnitPattern?, IBuildChainPattern>.Member4 => TunedNode;
}