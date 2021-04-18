using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.BuildActions;


namespace Armature
{
  /// <summary>
  ///   This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  /// <typeparam name="T">The type of parameter</typeparam>
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class ParameterValueTuner<T> : ParameterValueTuner
  {
    public ParameterValueTuner(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

    /// <summary>
    ///   Use the <paramref name="value" /> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue(T? value) => new(UnitPattern, new SingletonBuildAction(value), Weight);
  }
}
