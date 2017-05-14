using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class BuildingSugar
  {
    private readonly BuildStepBase _buildStep;
    private readonly BuildPlansCollection _container;

    public BuildingSugar(BuildStepBase buildStep, BuildPlansCollection container)
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
      return new BuildingSugar(_buildStep.AddOrGetBuildStep(buildStep), _container);
    }

    public TreatSugar<T> Treat<T>(object token = null)
    {
      var buildStep = new WeakBuildSequenceBuildStep(Match.Type<T>(token));
      return new TreatSugar<T>(_buildStep.AddOrGetBuildStep(buildStep));
    }

    public AdjusterSugar TreatAll()
    {
      var buildStep = new AnyUnitBuildStep();
      return new AdjusterSugar(_buildStep.AddOrGetBuildStep(buildStep));
    }
  }
}