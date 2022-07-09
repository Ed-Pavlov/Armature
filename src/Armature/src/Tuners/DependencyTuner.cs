using System;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public static class DependencyTuner
{
  public static T UsingArguments<T>(T tuner, params object[] arguments) where T : ITunerInternal
  {
    if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

    if(arguments.Any(arg => arg is null))
      throw new ArgumentNullException(
        nameof(arguments),
        $"Argument should be either {nameof(IArgumentSideTuner)} or a not null instance. "
      + $"Use {nameof(ForParameter)} or custom {nameof(IArgumentSideTuner)} to provide null as an argument for a parameter.");

    foreach(var argument in arguments)
      if(argument is IArgumentSideTuner argumentTuner)
        argumentTuner.Tune(tuner);
      else if(argument is ISideTuner)
        throw new ArgumentException($"{nameof(IArgumentSideTuner)} or instance expected");
      else
      {
        tuner.TreeRoot
        .GetOrAddNode(
                   new IfFirstUnit(
                     new IsAssignableFromType(argument.GetType()),
                     tuner.Weight //TODO: should the weight be added?
                   + WeightOf.InjectionPoint.ByTypeAssignability
                   + WeightOf.BuildChainPattern.TargetUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
                .TryAddContext(tuner)
                .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);
      }

    return tuner;
  }

  public static T InjectInto<T>(T tuner, params IInjectPointSideTuner[] propertyIds) where T : ITunerInternal
  {
    if(propertyIds is null) throw new ArgumentNullException(nameof(propertyIds));
    if(propertyIds.Length == 0) throw new ArgumentNullException(nameof(propertyIds), "Specify one or more inject point tuners");

    foreach(var injectPointTuner in propertyIds)
      injectPointTuner.Tune(tuner);

    return tuner;
  }
}
