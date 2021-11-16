using System;
using Armature.Core;


namespace Armature.Extensibility;

public abstract class BuildChainExtensibility : IBuildChainExtensibility
{
  protected readonly IBuildChainPattern ParentNode;

  protected BuildChainExtensibility(IBuildChainPattern parentNode) => ParentNode = parentNode ?? throw new ArgumentNullException(nameof(parentNode));

  IBuildChainPattern IBuildChainExtensibility.BuildChainPattern => ParentNode;
}