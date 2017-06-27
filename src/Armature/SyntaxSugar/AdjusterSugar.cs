using System;
using Armature.Core;
using Armature.Framework;
using Armature.Interface;

namespace Armature
{
  public class AdjusterSugar
  {
    private readonly StaticBuildStep _buildStep;

    public AdjusterSugar(StaticBuildStep buildStep)
    {
      _buildStep = buildStep;
    }

    /// <summary>
    /// The set of values for parameters of registered Unit can be values or implementation of <see cref="IParameterValueBuildPlanner"/>.
    /// See <see cref="For"/> for details 
    /// </summary>
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

    /// <summary>
    /// Register Unit as an eternal singleton <see cref="SingletonBuildAction"/> for details
    /// </summary>
    public void AsSingleton()
    {
      _buildStep.AddBuildAction(BuildStage.Cache, new SingletonBuildAction());
    }

    /// <summary>
    /// Instantiate an Unit using a constructor marked with <see cref="InjectAttribute"/>(<see cref="injectionPointId"/>)
    /// </summary>
    public AdjusterSugar UsingAttributedConstructor(object injectionPointId)
    {
      _buildStep.AddBuildStep(new FindAttributedConstructorBuildStep(0, injectionPointId));
      return this;
    }
  }
}