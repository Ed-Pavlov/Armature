﻿using System;
using System.Diagnostics;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

public class FinalTuner : TunerBase
{
  [PublicAPI]
  protected int Weight;

  [DebuggerStepThrough]
  public FinalTuner(IBuildChainPattern parentNode) : base(parentNode) { }

  /// <summary>
  /// Provides arguments to inject into building unit. See <see cref="ForParameter" /> for details.
  /// </summary>
  public FinalTuner UsingArguments(params object[] arguments)
  {
    if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

    if(arguments.Any(arg => arg is null))
      throw new ArgumentNullException(
        nameof(arguments),
        $"Argument should be either {nameof(IArgumentTuner)} or a not null instance. "
      + $"Use {nameof(ForParameter)} or custom {nameof(IArgumentTuner)} to provide null as an argument for a parameter.");

    foreach(var argument in arguments)
      if(argument is IArgumentTuner argumentTuner)
        argumentTuner.Tune(ParentNode, Weight);
      else if(argument is ITuner)
        throw new ArgumentException($"{nameof(IArgumentTuner)} or instance expected");
      else
        ParentNode
         .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
         .GetOrAddNode(
            new IfFirstUnit(
              new IsAssignableFromType(argument.GetType()),
              WeightOf.BuildChainPattern.IfFirstUnit + WeightOf.InjectionPoint.ByTypeAssignability + Weight))
         .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);

    return this;
  }

  public FinalTuner InjectInto(params IInjectPointTuner[] propertyIds)
  {
    if(propertyIds is null) throw new ArgumentNullException(nameof(propertyIds));
    if(propertyIds.Length == 0) throw new ArgumentNullException(nameof(propertyIds), "Specify one or more inject point tuners");

    foreach(var injectPointTuner in propertyIds)
      injectPointTuner.Tune(ParentNode, Weight);

    return this;
  }

  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/> using default implementation
  /// set to <see cref="Default.CreateSingletonBuildAction"/>.
  /// </summary>
  public void AsSingleton() => ParentNode.UseBuildAction(Default.CreateSingletonBuildAction(), BuildStage.Cache);

  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/>. See <see cref="Singleton" /> for details.
  /// </summary>
  public void AsSingletonSingleThread() => ParentNode.UseBuildAction(new Singleton(), BuildStage.Cache);

  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/> using thread safe version of the build action.
  /// See <see cref="ThreadSafeSingleton" /> for details.
  /// </summary>
  public void AsSingletonThreadSafe() => ParentNode.UseBuildAction(new ThreadSafeSingleton(), BuildStage.Cache);
}