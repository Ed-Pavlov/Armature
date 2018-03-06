﻿using System;
using System.Reflection;
 using Armature.Core;
using Armature.Framework;
 using Armature.Framework.BuildActions;
 using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public static class For
  {
    /// <summary>
    /// Matches with parameter with <see cref="ParameterInfo.ParameterType"/> equals to <see cref="T"/>
    /// </summary>
    public static ParameterValueBuildPlanner Parameter<T>(int weight = ParameterMatcherWeight.TypedParameter)
    {
      var matcher = new ParameterByStrictTypeMatcher(typeof(T));
      return new ParameterValueBuildPlanner(matcher, weight);
    }

    /// <summary>
    /// Matches with parameter with <see cref="ParameterInfo.Name"/> equals to <see cref="parameterName"/>
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <param name="weight">Weight of such match</param>
    /// <returns></returns>
    public static ParameterValueBuildPlanner ParameterName([NotNull] string parameterName, int weight = ParameterMatcherWeight.NamedParameter)
    {
      var matcher = new ParameterByNameMatcher(parameterName);
      return new ParameterValueBuildPlanner(matcher, weight);
    }

    /// <summary>
    /// Matches with parameter marked with <see cref="InjectAttribute"/>(<see cref="injectPointId"/>)
    /// </summary>
    /// <param name="injectPointId">Matches parameter marked with <see cref="InjectAttribute"/> with <see cref="InjectAttribute.InjectionPointId"/>
    /// equals to <paramref name="injectPointId"/></param>
    /// <param name="weight">Weight of such match</param>
    public static ParameterValueBuildPlanner ParameterId([CanBeNull] object injectPointId, int weight = ParameterMatcherWeight.AttributedParameter)
    {
      var matcher = new ParameterByInjectPointMatcher(injectPointId);
      return new ParameterValueBuildPlanner(matcher, weight);
    }

    public class ParameterValueBuildPlanner : IParameterValueBuildPlanner
    {
      private readonly IUnitMatcher _parameterMatcher;
      private readonly int _weight;

      private IBuildAction _buildAction;

      public ParameterValueBuildPlanner([NotNull] IUnitMatcher parameterMatcher, int weight)
      {
        if (parameterMatcher == null) throw new ArgumentNullException("parameterMatcher");
        _parameterMatcher = parameterMatcher;
        _weight = weight;
      }

      void IParameterValueBuildPlanner.AddBuildParameterValueStepTo(IUnitSequenceMatcher unitSequenceMatcher)
      {
        if(_buildAction == null) throw new InvalidOperationException("Parameter value source not specified, did you forget call UseValue/UseToken etc?");

        unitSequenceMatcher
          .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(_parameterMatcher, ParameterMatcherWeight.Lowest + 1))
          .AddBuildAction(BuildStage.Create, _buildAction, _weight);
      }

      /// <summary>
      /// Use the <see cref="value"/> for the parameter
      /// </summary>
      public IParameterValueBuildPlanner UseValue([CanBeNull] object value)
      {
        _buildAction = new SingletonBuildAction(value);
        return this;
      }

      /// <summary>
      /// For building a value for the parameter use <see cref="ParameterInfo.ParameterType"/> and <see cref="token"/>
      /// </summary>
      public IParameterValueBuildPlanner UseToken([NotNull] object token)
      {
        if (token == null) throw new ArgumentNullException("token");
        
        _buildAction = new RedirectParameterInfoBuildAction(token);
        return this;
      }

      public IParameterValueBuildPlanner UseResolver<T>(Func<UnitBuilder, T, object> resolver)
      {
        _buildAction = new CreateWithFactoryMethodBuildAction<T, object>(resolver);
        return this;
      }
    }
  }
}