using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;
using Armature.Framework;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public class AdjusterSugar : UnitSequenceExtensibility
  {

    [DebuggerStepThrough]
    public AdjusterSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {} 

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
        if (parameter is IParameterMatcherSugar parameterMatcher)
          parameterMatcher.AddBuildParameterValueStepTo(UnitSequenceMatcher);
        else
          UnitSequenceMatcher
            .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(new ParameterByWeakTypeMatcher(parameter), ParameterMatcherWeight.Lowest + 1))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(parameter), ParameterMatcherWeight.WeakTypedParameter);
      }

      return this;
    }

    /// <summary>
    ///   Register Unit as an eternal singleton <see cref="SingletonBuildAction" /> for details
    /// </summary>
    public void AsSingleton() => UnitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(), 0);

    /// <summary>
    ///   Instantiate an Unit using a constructor marked with <see cref="InjectAttribute" />(<see cref="injectionPointId" />)
    /// </summary>
    public AdjusterSugar UsingInjectPointConstructor(object injectionPointId)
    {
      var matcher = new ConstructorByInjectPointIdMatcher(injectionPointId);

      UnitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(matcher, 0))
        .AddBuildAction(BuildStage.Create, matcher.BuildAction, 0);
      return this;
    }

    public AdjusterSugar UsingParameterlessConstructor() => UsingConstructorWithParameters();

    public AdjusterSugar UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      var matcher = new ConstructorByParametersMatcher(parameterTypes);

      UnitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(matcher, 0))
        .AddBuildAction(BuildStage.Create, matcher.BuildAction, 0);
      return this;
    }

    public AdjusterSugar BuildingWhich([NotNull] Action<BuildingSugar> tuneAction)
    {
      if (tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new BuildingSugar(UnitSequenceMatcher));
      return this;
    }
  }
}