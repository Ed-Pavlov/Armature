﻿using Armature.Core;

namespace Armature.Extensibility;

/// <summary>
///   This interface is used to hide <see cref="BuildChainPattern" /> property from the intellisense list (inheritor should implement it explicitly)
///   but provides an access to the <see cref="IBuildChainPattern" /> for extensibility. See usages in Tests.Extensibility project.
/// </summary>
public interface IBuildChainExtensibility
{
  IBuildChainPattern BuildChainPattern { get; }
}