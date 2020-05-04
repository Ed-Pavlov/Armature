using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  /// <summary>
  ///   This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  /// <typeparam name="T">The type of parameter</typeparam>
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class ParameterValueTuner<T> : ParameterValueTuner
  {
    public ParameterValueTuner([NotNull] IUnitMatcher unitMatcher, int weight) : base(unitMatcher, weight) { }

    /// <summary>
    ///   Use the <paramref name="value" /> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue([CanBeNull] T value) => new ParameterValueBuildPlan(UnitMatcher, new SingletonBuildAction(value), Weight);
  }
}