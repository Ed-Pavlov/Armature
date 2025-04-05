using System;
using System.Collections.Generic;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Provides methods to configure argument resolution in different orders for dependency injection.
/// </summary>
public class Autowiring
{
  /// <summary>
  /// Configures arguments to be resolved in the same order as they are declared in the method or constructor.
  /// </summary>
  /// <returns>An <see cref="IInjectionPointSideTuner"/> to customize injection point behavior.</returns>
  public static IInjectionPointSideTuner ResolveArgumentsInDirectOrder()
    => new InjectionPointSideTuner(
      tuner =>
        tuner
         .GetTunerInternals()
         .TreeRoot
         .GetOrAddNode(new IfFirstUnit(new IsParameterInfoArray(), Core.WeightOf.BuildStackPattern.IfFirstUnit))
         .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create));

  /// <summary>
  /// Configures arguments to be resolved in reverse order of their declaration in the method or constructor.
  /// </summary>
  /// <returns>An <see cref="IInjectionPointSideTuner"/> to customize injection point behavior.</returns>
  public static IInjectionPointSideTuner ResolveArgumentsInReverseOrder()
    => new InjectionPointSideTuner(
      tuner =>
        tuner
         .GetTunerInternals()
         .TreeRoot
         .GetOrAddNode(new IfFirstUnit(new IsParameterInfoArray(), Core.WeightOf.BuildStackPattern.IfFirstUnit))
         .UseBuildAction(new BuildMethodArgumentsInReverseOrder(), BuildStage.Create));

  /// <summary>
  /// Configures arguments to be resolved in a custom order based on the provided reorder function.
  /// </summary>
  /// <param name="reorder">A function that specifies how arguments should be reordered for resolution.</param>
  /// <returns>An <see cref="IInjectionPointSideTuner"/> to customize injection point behavior.</returns>
  public static IInjectionPointSideTuner ResolveArgumentsInCustomOrder(Func<ParameterInfo[], IEnumerable<Tuple<int, ParameterInfo>>> reorder)
    => new InjectionPointSideTuner(
      tuner =>
        tuner
         .GetTunerInternals()
         .TreeRoot
         .GetOrAddNode(new IfFirstUnit(new IsParameterInfoArray(), Core.WeightOf.BuildStackPattern.IfFirstUnit))
         .UseBuildAction(new BuildMethodArgumentsInCustomOrder(reorder), BuildStage.Create));
}

/// <summary>
/// Provides methods to configure and customize argument resolution strategies for dependency injection.
/// </summary>
public class ArgumentResolver
{
  /// <summary>
  /// Attempts to resolve arguments using multiple strategies specified by the provided <paramref name="action"/>.
  /// </summary>
  /// <param name="action">A delegate to configure the build rules for argument resolution.</param>
  /// <returns>An <see cref="ISideTuner"/> to customize dependency injection behavior.</returns>
  public static ISideTuner TryInOrder(Action<Build> action)
  {
    var sugar = new Build();
    action(sugar);

    return new SideTuner(
      tuner => tuner
              .GetTunerInternals()
              .TreeRoot
              .AddNode(new IfFirstUnit(new IsParameterArgument()))
              .UseBuildAction(new TryInOrder(sugar._buildActions.ToArray()), BuildStage.Create));
  }

  /// <summary>
  /// Provides configuration options for specifying argument resolution strategies in <see cref="ArgumentResolver"/>.
  /// </summary>
  /// <remarks>
  /// It's named that to IDE suggest good name for parameter of action passed to <see cref="ArgumentResolver.TryInOrder"/>
  /// </remarks>
  public class Build
  {
    internal readonly List<IBuildAction> _buildActions = [];

    /// <summary>
    /// Attempts to resolve arguments using their parameters <see cref="InjectAttribute.Tag"/> as <see cref="UnitId.Tag"/>.
    /// </summary>
    public void ArgumentByParameterInjectPoint() => _buildActions.Add(new BuildArgumentByParameterInjectPoint());

    /// <summary>
    /// Attempts to resolve arguments by their parameter types.
    /// </summary>
    public void ArgumentByParameterType() => _buildActions.Add(new BuildArgumentByParameterType());

    /// <summary>
    /// Attempts to resolve arguments using their parameter names as <see cref="UnitId.Kind"/>.
    /// </summary>
    public void ArgumentByParameterName() => _buildActions.Add(new BuildArgumentByParameterName());

    /// <summary>
    /// Uses the default value of the parameter as its argument if a value is not explicitly provided.
    /// </summary>
    public void ParameterDefaultValue() => _buildActions.Add(new GetParameterDefaultValue());
  }
}
