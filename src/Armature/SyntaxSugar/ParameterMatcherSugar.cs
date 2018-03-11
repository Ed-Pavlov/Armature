using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  public class ParameterMatcherSugar : IParameterMatcherSugar
  {
    private readonly IUnitMatcher _parameterMatcher;
    private readonly int _weight;
    private IBuildAction _buildAction;

    [DebuggerStepThrough]
    public ParameterMatcherSugar([NotNull] IUnitMatcher parameterMatcher, int weight)
    {
      _parameterMatcher = parameterMatcher ?? throw new ArgumentNullException(nameof(parameterMatcher));
      _weight = weight;
    }

    IBuildAction IParameterMatcherSugar.BuildAction { [DebuggerStepThrough] get => _buildAction; [DebuggerStepThrough] set => _buildAction = value; }

    void IParameterMatcherSugar.AddBuildParameterValueStepTo(IUnitSequenceMatcher unitSequenceMatcher)
    {
      if (_buildAction == null) throw new InvalidOperationException("Parameter value source not specified, did you forget call UseValue/UseToken etc?");

      unitSequenceMatcher
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(_parameterMatcher, ParameterMatcherWeight.Lowest + 1))
        .AddBuildAction(BuildStage.Create, _buildAction, _weight);
    }

    /// <summary>
    ///   Use the <see cref="value" /> for the parameter
    /// </summary>
    public IParameterMatcherSugar UseValue([CanBeNull] object value)
    {
      _buildAction = new SingletonBuildAction(value);
      return this;
    }

    /// <summary>
    ///   For building a value for the parameter use <see cref="ParameterInfo.ParameterType" /> and <see cref="token" />
    /// </summary>
    public IParameterMatcherSugar UseToken([NotNull] object token)
    {
      if (token == null) throw new ArgumentNullException(nameof(token));

      _buildAction = new RedirectParameterInfoBuildAction(token);
      return this;
    }

    public IParameterMatcherSugar UseResolver<T>(Func<UnitBuilder, T, object> resolver)
    {
      _buildAction = new CreateWithFactoryMethodBuildAction<T, object>(resolver);
      return this;
    }
  }

  /// <summary>
  ///   This generic type is used for further extensibility possibilities which involves generic types. Generic type can't be constructed from typeof
  /// </summary>
  /// <typeparam name="T">The type of parameter</typeparam>
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public class ParameterMatcherSugar<T> : ParameterMatcherSugar
  {
    public ParameterMatcherSugar([NotNull] IUnitMatcher parameterMatcher, int weight) : base(parameterMatcher, weight) { }
  }
}