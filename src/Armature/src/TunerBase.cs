using System;
using Armature.Core;
using Armature.Extensibility;

namespace Armature;

public abstract class TunerBase : IInternal<IBuildChainPattern>
{
  protected readonly IBuildChainPattern ParentNode;

  protected TunerBase(IBuildChainPattern parentNode) => ParentNode = parentNode ?? throw new ArgumentNullException(nameof(parentNode));

  IBuildChainPattern IInternal<IBuildChainPattern>.Member1 => ParentNode;
}
