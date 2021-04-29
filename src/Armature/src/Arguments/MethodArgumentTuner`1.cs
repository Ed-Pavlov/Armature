using Armature.Core;

namespace Armature
{
  /// <summary>
  ///   This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  public class MethodArgumentTuner<T> : MethodArgumentTuner
  {
    public MethodArgumentTuner(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

    /// <summary>
    ///   Use the <paramref name="value" /> for the parameter
    /// </summary>
    public ITuner UseValue(T? value) => new TunerImpl(UnitPattern, new Singleton(value), Weight);
  }
}
