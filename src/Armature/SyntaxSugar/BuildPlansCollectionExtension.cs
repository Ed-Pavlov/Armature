using System;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    public static TreatSugar<T> Treat<T>([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException("container");

      var buildStep = new WeakBuildSequenceBuildStep(Match.Type<T>(token));
      return new TreatSugar<T>(container.AddOrGetBuildStep(buildStep));
    }

    public static TreatOpenGenericSugar TreatOpenGeneric([NotNull] this BuildPlansCollection container, Type openGenericType, object token = null)
    {
      if (container == null) throw new ArgumentNullException("container");

      var buildStep = new WeakBuildSequenceBuildStep(Match.OpenGenericType(openGenericType, token));
      return new TreatOpenGenericSugar(container.AddOrGetBuildStep(buildStep));
    }

    public static AdjusterSugar TreatAll([NotNull] this BuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException("container");

      var buildStep = new AnyUnitBuildStep();
      return new AdjusterSugar(container.AddOrGetBuildStep(buildStep));
    }

    public static BuildingSugar Building<T>(this BuildPlansCollection container, object token = null)
    {
      return container.Building(typeof(T), token);
    }

    public static BuildingSugar Building([NotNull] this BuildPlansCollection container, Type type, object token = null)
    {
      if (container == null) throw new ArgumentNullException("container");

      var buildStep = new WeakBuildSequenceBuildStep(Match.Type(type, token));
      return new BuildingSugar(container.AddOrGetBuildStep(buildStep), container);
    }
  }
}