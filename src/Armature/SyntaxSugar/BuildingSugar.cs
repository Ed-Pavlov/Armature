using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class BuildingSugar
  {
    private readonly WeakBuildSequenceBuildStep _buildStep;
    private readonly IBuildPlansCollection _container;

    public BuildingSugar(WeakBuildSequenceBuildStep buildStep, IBuildPlansCollection container)
    {
      _buildStep = buildStep;
      _container = container;
    }

    public BuildingSugar Building<T>(object token = null)
    {
      return Building(typeof(T), token);
    }

    public BuildingSugar Building([NotNull] Type type, object token = null)
    {
      if (type == null) throw new ArgumentNullException("type");
      var buildStep = new WeakBuildSequenceBuildStep(Match.Type(type, token));
      _buildStep.AddChildBuildStep(buildStep);
      return new BuildingSugar(buildStep, _container);
    }

    public TreatSugar<T> Treat<T>(object token = null)
    {
      var buildStep = new WeakBuildSequenceBuildStep(Match.Type<T>(token));
      _buildStep.AddChildBuildStep(buildStep);
      return new TreatSugar<T>(buildStep);
    }

    public AdjusterSugar TreatAll()
    {
      var buildStep = new AnyUnitBuildStep();
      _buildStep.AddChildBuildStep(buildStep);
      return new AdjusterSugar(buildStep);
    }
  }
}