using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public delegate IBuildChainPattern AddContextPatterns(IBuildChainPattern node);

public abstract class TunerBase : IInternal<IBuildChainPattern, IBuildChainPattern, AddContextPatterns?, IUnitPattern?>
{
  protected readonly IBuildChainPattern TreeRoot;
  protected readonly IBuildChainPattern TunedNode;

  protected readonly AddContextPatterns? ContextFactory;

  protected readonly IUnitPattern? UnitPattern;

  protected int Weight;

  protected TunerBase(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns? contextFactory, IUnitPattern? unitPattern)
  {
    TreeRoot       = treeRoot  ?? throw new ArgumentNullException(nameof(treeRoot));
    TunedNode      = tunedNode ?? throw new ArgumentNullException(nameof(tunedNode));
    ContextFactory = contextFactory;
    UnitPattern    = unitPattern;
  }

  IBuildChainPattern IInternal<IBuildChainPattern>.                                                   Member1 => TunedNode;
  IBuildChainPattern IInternal<IBuildChainPattern, IBuildChainPattern>.                               Member2 => TreeRoot;
  AddContextPatterns? IInternal<IBuildChainPattern, IBuildChainPattern, AddContextPatterns?>.         Member3 => ContextFactory;
  IUnitPattern? IInternal<IBuildChainPattern, IBuildChainPattern, AddContextPatterns?, IUnitPattern?>.Member4 => UnitPattern;
}