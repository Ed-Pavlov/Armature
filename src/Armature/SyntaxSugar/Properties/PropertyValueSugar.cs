using System;
using System.Reflection;
using Armature.Core;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Creation;
using Armature.Framework.BuildActions.Property;
using Armature.Interface;
using Armature.Properties;

namespace Armature
{
  public class PropertyValueSugar
  {
    private readonly IUnitMatcher _propertyMatcher;
    private readonly IBuildAction _getPropertyAction;
    private readonly int _weight;

    public PropertyValueSugar([NotNull] IUnitMatcher propertyMatcher, [NotNull] IBuildAction getPropertyAction, int weight)
    {
      if (propertyMatcher is null) throw new ArgumentNullException(nameof(propertyMatcher));
      if (getPropertyAction is null) throw new ArgumentNullException(nameof(getPropertyAction));

      _propertyMatcher = propertyMatcher;
      _getPropertyAction = getPropertyAction;
      _weight = weight;
    }

    /// <summary>
    ///   Inject the <see cref="value" /> into the property
    /// </summary>
    public PropertyValueBuildPlan UseValue([CanBeNull] object value) => new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new SingletonBuildAction(value), _weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="token" />
    /// </summary>
    public PropertyValueBuildPlan UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));
      return new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new RedirectPropertyToTypeAndTokenBuildAction(token), _weight);
    }

    /// <summary>
    ///   For building a value for the property use factory method />
    /// </summary>
    public PropertyValueBuildPlan UseResolver(Func<IBuildSession, object> resolver) => 
      new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new CreateWithFactoryMethodBuildAction<object>(resolver), _weight);
    
    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="InjectAttribute.InjectionPointId"/> as a token />
    /// </summary>
    public PropertyValueBuildPlan UseInjectPointIdAsToken() => new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, RedirectPropertyInjectPointToTypeAndTokenBuildAction.Instance, _weight);
  }
  
  public class PropertyValueSugar<T> : PropertyValueSugar
  {
    public PropertyValueSugar([NotNull] IUnitMatcher propertyMatcher, IBuildAction getPropertyAction, int weight) : base(propertyMatcher, getPropertyAction, weight)
    {
    }
  }
}