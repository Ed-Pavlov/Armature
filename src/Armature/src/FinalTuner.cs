using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Extensibility;

namespace Armature;

public class FinalTuner : BuildChainExtensibility
{
  [DebuggerStepThrough]
  public FinalTuner(IBuildChainPattern parentNode) : base(parentNode) { }

  /// <summary>
  ///   Provides arguments to inject into building unit. See <see cref="ForParameter" /> for details.
  /// </summary>
  public FinalTuner UsingArguments(params object[] arguments)
  {
    if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

    foreach(var argument in arguments)
      if(argument is IArgumentTuner buildPlan)
        buildPlan.Tune(ParentNode);
      else if(argument is ITuner)
        throw new ArgumentException($"{nameof(IArgumentTuner)} or instances expected");
      else
        ParentNode
         .GetOrAddNode(new SkipWhileUnitBuildChain(Static.Of<IsServiceUnit>(), 0))
         .GetOrAddNode(new IfFirstUnitBuildChain(new IsAssignableFromType(argument.GetType()), WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByTypeAssignability))
         .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);

    return this;
  }

  public FinalTuner InjectInto(params IInjectPointTuner[] propertyIds)
  {
    if(propertyIds.Length == 0) throw new ArgumentNullException(nameof(propertyIds), "Specify one or more inject point tuners");
    foreach(var injectPointTuner in propertyIds)
      injectPointTuner.Tune(ParentNode);
    return this;
  }

  /// <summary>
  ///   Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/>. See <see cref="Singleton" /> for details
  /// </summary>
  public void AsSingleton() => ParentNode.UseBuildAction(new Singleton(), BuildStage.Cache);
}