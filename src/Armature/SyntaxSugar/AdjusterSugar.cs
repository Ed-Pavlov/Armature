using System;
using Armature.Core;
using Armature.Framework;

namespace Armature
{
  public class AdjusterSugar
  {
    private readonly BuildStep _buildStep;

    public AdjusterSugar(BuildStep buildStep)
    {
      _buildStep = buildStep;
    }

    public AdjusterSugar UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
      {
        var parameterBuildPlanner = parameter as IParameterBuildPlanner;
        if (parameterBuildPlanner != null)
          parameterBuildPlanner.RegisterParameterResolver(_buildStep);
        else
          _buildStep.AddChildBuildStep(new WeakParameterTypeValueBuildStep(parameter));
      }
      return this;
    }

    public void AsSingleton()
    {
      _buildStep.AddBuildAction(BuildStage.Cache, new SingletonBuildAction());
    }

    public AdjusterSugar UsingAttributedConstructor(object injectionPointId)
    {
      _buildStep.AddChildBuildStep(new FindAttributedConstructorBuildStep(0, injectionPointId));
      return this;
    }
  }
}