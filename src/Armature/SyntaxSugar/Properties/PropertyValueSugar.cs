using System;
using System.Reflection;
using Armature.Core;
using Armature.Framework.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  public class PropertyValueSugar
  {
    private readonly IUnitMatcher _propertyMatcher;
    private readonly IBuildAction _getPropertyAction;
    private readonly int _weight;

    public PropertyValueSugar([NotNull] IUnitMatcher propertyMatcher, IBuildAction getPropertyAction, int weight)
    {
      if (propertyMatcher is null) throw new ArgumentNullException(nameof(propertyMatcher));

      _propertyMatcher = propertyMatcher;
      _getPropertyAction = getPropertyAction;
      _weight = weight;
    }

    /// <summary>
    ///   Use the <see cref="value" /> for the parameter
    /// </summary>
    public PropertyValueBuildPlan UseValue([CanBeNull] object value) => new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new SingletonBuildAction(value), _weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="token" />
    /// </summary>
    public PropertyValueBuildPlan UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));
      return new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new RedirectPropertyToTypeAndTokenBuildAction(token), _weight);
    }

    public PropertyValueBuildPlan UseResolver(Func<IBuildSession, object> resolver) => 
      new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, new CreateWithFactoryMethodBuildAction<object>(resolver), _weight);
    
    public PropertyValueBuildPlan UseInjectPointIdAsToken() => new PropertyValueBuildPlan(_propertyMatcher, _getPropertyAction, RedirectPropertyInjectPointToTypeAndTokenBuildAction.Instance, _weight);
  }
  
  public class PropertyValueSugar<T> : PropertyValueSugar
  {
    public PropertyValueSugar([NotNull] IUnitMatcher propertyMatcher, IBuildAction getPropertyAction, int weight) : base(propertyMatcher, getPropertyAction, weight)
    {
    }
  }
}