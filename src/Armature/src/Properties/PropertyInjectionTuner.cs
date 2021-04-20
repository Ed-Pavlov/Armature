using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class PropertyInjectionTuner : BuildActionExtensibility
  {
    public PropertyInjectionTuner(IUnitPattern unitIsProperty, IBuildAction getPropertyAction, int weight)
      : base(unitIsProperty, getPropertyAction, weight) { }

    /// <summary>
    ///   Inject the <paramref name="value" /> into the property
    /// </summary>
    public ITuner UseValue(object? value) => new ArgumentForPropertyTuner(UnitPattern, BuildAction, new Singleton(value), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <paramref name="key" />
    /// </summary>
    public ITuner UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new ArgumentForPropertyTuner(UnitPattern, BuildAction, new BuildArgumentForProperty(key), Weight);
    }

    /// <summary>
    ///   For building a value for the property use factory method />
    /// </summary>
    public ITuner UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new ArgumentForPropertyTuner(UnitPattern, BuildAction, new CreateWithFactoryMethod<object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a key
    /// </summary>
    public ITuner UseInjectPointIdAsKey() => new ArgumentForPropertyTuner(UnitPattern, BuildAction, BuildArgumentForPropertyWithInjectPointIdAsKey.Instance, Weight);
  }

  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class PropertyInjectionTuner<T> : PropertyInjectionTuner
  {
    public PropertyInjectionTuner(IUnitPattern unitIsPropertyInfo, IBuildAction getPropertyAction, int weight)
      : base(unitIsPropertyInfo, getPropertyAction, weight) { }
  }
}
