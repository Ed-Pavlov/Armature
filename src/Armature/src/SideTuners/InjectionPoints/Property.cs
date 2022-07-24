using System;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Tunes up in which properties of the object inject dependencies.
/// </summary>
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
        tuner.CreateContextBranch().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        tuner.TreeRoot
             .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>()))
             .AppendContextBranch(tuner)
             .UseBuildAction(new GetPropertyByType(type), BuildStage.Create);
      });

  /// <summary>
  /// Sets up the properties with names as in the passed <paramref name="names"/> list as required a dependency to be injected into it.
  /// </summary>
  [PublicAPI]
  public static IInjectionPointSideTuner Named(params string[] names)
  {
    if(names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));
    if(names.Any(string.IsNullOrEmpty)) throw new ArgumentNullException(nameof(names), "One or more items are null or empty string.");

    return new InjectionPointSideTuner(
      tuner =>
      {
        tuner.CreateContextBranch().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        tuner.TreeRoot
             .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>()))
             .AppendContextBranch(tuner)
             .UseBuildAction(new GetPropertyListByNames(names), BuildStage.Create);
      });
  }

  /// <summary>
  /// Sets up the properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="pointIds" />
  /// as required a dependency to be injected into it.
  /// </summary>
  [PublicAPI]
  public static IInjectionPointSideTuner ByInjectPointId(params object?[] pointIds)
    => new InjectionPointSideTuner(
      tuner =>
      {
        tuner.CreateContextBranch().UseBuildAction(Static.Of<InjectDependenciesIntoProperties>(), BuildStage.Initialize);

        tuner.TreeRoot
             .GetOrAddNode(new IfFirstUnit(Static.Of<IsPropertyInfoCollection>()))
             .AppendContextBranch(tuner)
             .UseBuildAction(new GetPropertyListByInjectPointId(pointIds), BuildStage.Create);
      });
}