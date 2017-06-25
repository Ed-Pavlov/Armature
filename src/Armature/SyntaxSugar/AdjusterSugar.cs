using System;
using Armature.Core;
using Armature.Framework;

namespace Armature
{
  public class AdjusterSugar
  {
    private readonly StaticBuildStep _buildStep;

    public AdjusterSugar(StaticBuildStep buildStep)
    {
      _buildStep = buildStep;
    }

    public AdjusterSugar UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
      {
        var parameterBuildPlanner = parameter as IParameterValueBuildPlanner;
        if (parameterBuildPlanner != null)
          parameterBuildPlanner.AddBuildParameterValueStepTo(_buildStep);
        else
          _buildStep.AddBuildStep(new WeakParameterTypeValueBuildStep(ParameterValueBuildActionWeight.FreeValueResolver, parameter));
      }
      return this;
    }

    public void AsSingleton()
    {
      _buildStep.AddBuildAction(BuildStage.Cache, new SingletonBuildAction());
    }

    public AdjusterSugar UsingAttributedConstructor(object injectionPointId)
    {
      _buildStep.AddBuildStep(new FindAttributedConstructorBuildStep(0, injectionPointId));
      return this;
    }
  }
}