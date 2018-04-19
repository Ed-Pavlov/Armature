using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.BuildActions.Property;
using Resharper.Annotations;

namespace Armature
{
  public class PropertyValueTuner
  {
    private readonly IBuildAction _getPropertyAction;
    private readonly IUnitMatcher _propertyMatcher;
    private readonly int _weight;

    public PropertyValueTuner([NotNull] IUnitMatcher propertyMatcher, [NotNull] IBuildAction getPropertyAction, int weight)
    {
      if (propertyMatcher is null) throw new ArgumentNullException(nameof(propertyMatcher));
      if (getPropertyAction is null) throw new ArgumentNullException(nameof(getPropertyAction));

      _propertyMatcher = propertyMatcher;
      _getPropertyAction = getPropertyAction;
      _weight = weight;
    }

    /// <summary>
    ///   Inject the <paramref name="value" /> into the property
    /// </summary>
    public PropertyValueBuildPlan UseValue([CanBeNull] object value) => 
      new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new SingletonBuildAction(value), _weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <paramref name="token"/>
    /// </summary>
    public PropertyValueBuildPlan UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));

      return new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new CreatePropertyValueBuildAction(token), _weight);
    }

    /// <summary>
    ///   For building a value for the property use factory method />
    /// </summary>
    public PropertyValueBuildPlan UseFactoryMethod(Func<IBuildSession, object> factoryMethod) =>
      new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new CreateByFactoryMethodBuildAction<object>(factoryMethod), _weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a token
    /// </summary>
    public PropertyValueBuildPlan UseInjectPointIdAsToken() => 
      new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, CreatePropertyValueForInjectPointBuildAction.Instance, _weight);
  }

  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class PropertyValueTuner<T> : PropertyValueTuner
  {
    public PropertyValueTuner([NotNull] IUnitMatcher propertyMatcher, IBuildAction getPropertyAction, int weight) 
      : base(propertyMatcher, getPropertyAction, weight)
    {
    }
  }
}