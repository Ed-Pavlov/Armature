using System;
using System.Linq;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;
using WeightOf = BeatyBit.Armature.Core.WeightOf;

namespace BeatyBit.Armature;

/// <summary>
/// Tunes up in which properties of the object inject dependencies.
/// </summary>
[PublicAPI]
public static class Property
{
  /// <summary>
  /// Sets up the property of type <typeparamref name="T"/> as required a dependency to be injected into it.
  /// </summary>
  public static IInjectionPointSideTuner OfType<T>() => OfType(typeof(T));

  /// <summary>
  /// Sets up the property of type <paramref name="type"/>> as required a dependency to be injected into it.
  /// </summary>
  public static IInjectionPointSideTuner OfType(Type type)
    => new InjectionPointSideTuner(
      tuner =>
      {
        var internals = tuner.GetTunerInternals();
        internals.Apply().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        internals.TreeRoot
                 .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>(), WeightOf.BuildStackPattern.IfFirstUnit))
                 .ApplyTuner(tuner)
                 .UseBuildAction(new GetPropertyByType(type), BuildStage.Create);
      });

  /// <summary>
  /// Sets up the properties with names passed with <paramref name="names"/> list as required a dependency to be injected into it.
  /// </summary>
  [PublicAPI]
  public static IInjectionPointSideTuner Named(params string[] names)
  {
    if(names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));
    if(names.Any(string.IsNullOrEmpty)) throw new ArgumentNullException(nameof(names), "One or more items are null or empty string.");

    return new InjectionPointSideTuner(
      tuner =>
      {
        var internals = tuner.GetTunerInternals();
        internals.Apply().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        internals.TreeRoot
                 .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>(), WeightOf.BuildStackPattern.IfFirstUnit))
                 .ApplyTuner(tuner)
                 .UseBuildAction(new GetPropertyListByNames(names), BuildStage.Create);
      });
  }

  /// <summary>
  /// Sets up the properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="tags" />
  /// as required a dependency to be injected into it. /// </summary>
  public static IInjectionPointSideTuner ByInjectPointTag(params object?[] tags)
    => new InjectionPointSideTuner(
      tuner =>
      {
        var internals = tuner.GetTunerInternals();
        internals.Apply().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        internals.TreeRoot
                 .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>(), WeightOf.BuildStackPattern.IfFirstUnit))
                 .ApplyTuner(tuner)
                 .UseBuildAction(new GetPropertyListByTags(tags), BuildStage.Create);
      });
}
