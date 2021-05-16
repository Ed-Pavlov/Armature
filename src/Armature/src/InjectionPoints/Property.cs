using System;
using Armature.Core;

namespace Armature
{
  public static class Property
  {
    public static IInjectPointTuner OfType<T>(short weight = 0) => OfType(typeof(T), weight);

    public static IInjectPointTuner OfType(Type type, short weight = 0)
      => new InjectPointTuner(
        node =>
        {
          node.UseBuildAction(Static<InjectDependenciesIntoProperties>.Instance, BuildStage.Initialize);

          node.GetOrAddNode(new IfFirstUnit(Static<IsPropertyList>.Instance, weight))
              .UseBuildAction(new GetPropertyByType(type), BuildStage.Create);
        });

    /// <summary>
    ///   Adds a plan injecting dependencies into properties with corresponding <paramref name="names" />
    /// </summary>
    public static IInjectPointTuner Named(params string[] names) => Named(0, names);

    /// <summary>
    ///   Adds a plan injecting dependencies into properties with corresponding <paramref name="names" />
    /// </summary>
    public static IInjectPointTuner Named(short weight, params string[] names)
      => new InjectPointTuner(
        node =>
        {
          node.UseBuildAction(Static<InjectDependenciesIntoProperties>.Instance, BuildStage.Initialize);

          node.GetOrAddNode(new IfFirstUnit(Static<IsPropertyList>.Instance, weight))
              .UseBuildAction(new GetPropertyListByNames(names), BuildStage.Create);
        });

    /// <summary>
    ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="pointIds" />
    /// </summary>
    public static IInjectPointTuner ByInjectPoint(params object?[] pointIds) => ByInjectPoint(0, pointIds);

    /// <summary>
    ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding <paramref name="pointIds" />
    /// </summary>
    public static IInjectPointTuner ByInjectPoint(short weight, params object?[] pointIds)
      => new InjectPointTuner(
        node =>
        {
          node.UseBuildAction(Static<InjectDependenciesIntoProperties>.Instance, BuildStage.Initialize);

          node.GetOrAddNode(new IfFirstUnit(Static<IsPropertyList>.Instance, weight))
              .UseBuildAction(new GetPropertyListByInjectPointId(pointIds), BuildStage.Create);
        });
  }
}
