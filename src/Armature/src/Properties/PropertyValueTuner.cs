using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.BuildActions.Property;
using Armature.Extensibility;


namespace Armature
{
  public class PropertyValueTuner : BuildActionExtensibility
  {
    public PropertyValueTuner(IUnitPattern propertyUnitPattern, IBuildAction getPropertyAction, int weight)
      : base(propertyUnitPattern, getPropertyAction, weight) { }

    /// <summary>
    ///   Inject the <paramref name="value" /> into the property
    /// </summary>
    public PropertyValueBuildPlan UseValue(object? value) => new(UnitPattern, BuildAction, new SingletonBuildAction(value), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <paramref name="key" />
    /// </summary>
    public PropertyValueBuildPlan UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new PropertyValueBuildPlan(UnitPattern, BuildAction, new CreatePropertyValueBuildAction(key), Weight);
    }

    /// <summary>
    ///   For building a value for the property use factory method />
    /// </summary>
    public PropertyValueBuildPlan UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new(UnitPattern, BuildAction, new CreateByFactoryMethodBuildAction<object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a key
    /// </summary>
    public PropertyValueBuildPlan UseInjectPointIdAsKey() => new(UnitPattern, BuildAction, CreatePropertyValueForInjectPointBuildAction.Instance, Weight);
  }

  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class PropertyValueTuner<T> : PropertyValueTuner
  {
    public PropertyValueTuner(IUnitPattern propertyUnitPattern, IBuildAction getPropertyAction, int weight)
      : base(propertyUnitPattern, getPropertyAction, weight) { }
  }
}
