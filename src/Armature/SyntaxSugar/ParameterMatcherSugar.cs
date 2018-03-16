using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Framework.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  public class ParameterMatcherSugar
  {
    private readonly IUnitMatcher _parameterMatcher;
    private readonly int _weight;

    [DebuggerStepThrough]
    public ParameterMatcherSugar([NotNull] IUnitMatcher parameterMatcher, int weight)
    {
      _parameterMatcher = parameterMatcher ?? throw new ArgumentNullException(nameof(parameterMatcher));
      _weight = weight;
    }

    /// <summary>
    ///   Use the <see cref="value" /> for the parameter
    /// </summary>
    public ParameterValueBuildPlan UseValue([CanBeNull] object value) => new ParameterValueBuildPlan(_parameterMatcher, new SingletonBuildAction(value), _weight);

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="token" />
    /// </summary>
    public ParameterValueBuildPlan UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));
      return new ParameterValueBuildPlan(_parameterMatcher, new RedirectParameterInfoBuildAction(token), _weight);
    }

    public ParameterValueBuildPlan UseResolver<T>(Func<IBuildSession, T, object> resolver) => 
      new ParameterValueBuildPlan(_parameterMatcher, new CreateWithFactoryMethodBuildAction<T, object>(resolver), _weight);
    
    public ParameterValueBuildPlan UseInjectPointIdAsToken() => new ParameterValueBuildPlan(_parameterMatcher, RedirectParameterInfoToTypeAndTokenBuildAction.Instance, _weight);
  }

  /// <summary>
  /// This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  /// <typeparam name="T">The type of parameter</typeparam>
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class ParameterMatcherSugar<T> : ParameterMatcherSugar
  {
    public ParameterMatcherSugar([NotNull] IUnitMatcher parameterMatcher, int weight) : base(parameterMatcher, weight) { }
  }
}