using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Framework;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public class AdjusterSugar : ISugar
  {
    private readonly IUnitSequenceMatcher _unitSequenceMatcher;
    protected readonly BuildPlansCollection Container;

    [DebuggerStepThrough]
    public AdjusterSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [NotNull] BuildPlansCollection container)
    {
      if (unitSequenceMatcher == null) throw new ArgumentNullException(nameof(unitSequenceMatcher));
      if (container == null) throw new ArgumentNullException(nameof(container));

      _unitSequenceMatcher = unitSequenceMatcher;
      Container = container;
    }

    BuildPlansCollection ISugar.BuildPlansCollection => Container;

    /// <summary>
    ///   The set of values for parameters of registered Unit can be values or implementation of <see cref="IParameterMatcherSugar" />.
    ///   See <see cref="For" /> for details
    /// </summary>
    public AdjusterSugar UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
      {
        var parameterBuildPlanner = parameter as IParameterMatcherSugar;
        if (parameterBuildPlanner != null)
          parameterBuildPlanner.AddBuildParameterValueStepTo(_unitSequenceMatcher);
        else
          _unitSequenceMatcher
            .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(new ParameterByWeakTypeMatcher(parameter), ParameterMatcherWeight.Lowest + 1))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(parameter), ParameterMatcherWeight.WeakTypedParameter);
      }

      return this;
    }

    /// <summary>
    ///   Register Unit as an eternal singleton <see cref="SingletonBuildAction" /> for details
    /// </summary>
    public void AsSingleton() => _unitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(), 0);

    /// <summary>
    ///   Instantiate an Unit using a constructor marked with <see cref="InjectAttribute" />(<see cref="injectionPointId" />)
    /// </summary>
    public AdjusterSugar UsingInjectPointConstructor(object injectionPointId)
    {
      var matcher = new ConstructorByInjectPointIdMatcher(injectionPointId);

      _unitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(matcher, 0))
        .AddBuildAction(BuildStage.Create, matcher.BuildAction, 0);
      return this;
    }

    public AdjusterSugar UsingParameterlessConstructor() => UsingConstructorWithParameters();

    public AdjusterSugar UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      var matcher = new ConstructorByParametersMatcher(parameterTypes);

      _unitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(matcher, 0))
        .AddBuildAction(BuildStage.Create, matcher.BuildAction, 0);
      return this;
    }
  }
}