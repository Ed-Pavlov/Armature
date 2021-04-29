using Armature.Core;

namespace Armature
{
  public class PropertyArgumentTuner<T> : PropertyArgumentTuner
  {
    public PropertyArgumentTuner(IUnitPattern unitIsPropertyInfo, IBuildAction getPropertyAction, int weight)
      : base(unitIsPropertyInfo, getPropertyAction, weight) { }
      
    /// <summary>
    ///   Inject the <paramref name="value" /> into the property
    /// </summary>
    public IArgumentTuner UseValue(T value) => new TunerImpl(UnitPattern, BuildAction, new Singleton(value), Weight);
  }
}
