using System;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public static class BuildPlansCollectionExtension
  {
    public static TreatSugar<T> Treat<T>(this IBuildPlansCollection container, object token = null)
    {
      var buildStep = new WeakBuildSequenceBuildStep(Match.Type<T>(token));
      container.AddBuildStep(buildStep);
      return new TreatSugar<T>(buildStep);
    }

    public static TreatOpenGenericSugar TreatOpenGeneric(this IBuildPlansCollection container, Type openGenericType, object token = null)
    {
      var buildStep = new WeakBuildSequenceBuildStep(Match.OpenGenericType(openGenericType, token));
      container.AddBuildStep(buildStep);
      return new TreatOpenGenericSugar(buildStep);
    }

    public static AdjusterSugar TreatAll([NotNull] this IBuildPlansCollection container, object token = null)
    {
      if (container == null) throw new ArgumentNullException("container");
      var buildStep = new AnyUnitBuildStep();
      container.AddBuildStep(buildStep);
      return new AdjusterSugar(buildStep);
    }

    public static BuildingSugar Building<T>(this IBuildPlansCollection container, object token = null)
    {
      return container.Building(typeof(T), token);
    }

    public static BuildingSugar Building(this IBuildPlansCollection container, Type type, object token = null)
    {
      var weakBuildSequenceBuildStep = new WeakBuildSequenceBuildStep(Match.Type(type, token));
      var existBuildStep = (WeakBuildSequenceBuildStep)container.GetBuildStep(ArrayTail.Of(new IBuildStep[]{weakBuildSequenceBuildStep}, 0));

      if (existBuildStep == null)
      {
        container.AddBuildStep(weakBuildSequenceBuildStep);
        existBuildStep = weakBuildSequenceBuildStep;
      }

      return new BuildingSugar(existBuildStep, container);
    }
  }
}