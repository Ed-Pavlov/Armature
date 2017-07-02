using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Build step matches with <see cref="UnitInfo"/> describes value needed to pass to the parameter marked with special <see cref="Attribute"/> 
  /// <see cref="InjectAttribute"/> is used by default  by Armature framework.
  /// Provide your own <see cref=".ctor(Predicate{Attribute})"/> if you want to use another attribute.
  /// </summary>
  public class AttributedParameterValueBuildStep : ParameterValueBuildStep
  {
    private readonly Predicate<Attribute> _predicate;

    /// <summary>
    /// Creates build step which matches with parameter marked with <see cref="InjectAttribute"/> with <see cref="injectPointId"/> as inject point id
    /// </summary>
    /// <param name="matchingWeight"></param>
    /// <param name="injectPointId"></param>
    /// <param name="getBuildAction">Factory method returning build action for passed <see cref="ParameterInfo"/></param>
    public AttributedParameterValueBuildStep(
      int matchingWeight, 
      [CanBeNull] object injectPointId, 
      [NotNull] Func<ParameterInfo, IBuildAction> getBuildAction)
      : this(matchingWeight, CreateInjectAttributePredicate(injectPointId), getBuildAction)
    {}

    /// <summary>
    /// Use this constructor to use <see cref="AttributedParameterValueBuildStep"/> with any other attribute rether then <see cref="InjectAttribute"/>
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public AttributedParameterValueBuildStep(
      int matchingWeight, 
      [NotNull] Predicate<Attribute> predicate, 
      [NotNull] Func<ParameterInfo, IBuildAction> getBuildAction)
      : base(getBuildAction, matchingWeight)
    {
      if (predicate == null) throw new ArgumentNullException("predicate");
      _predicate = predicate;
    }

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      var injectAttribute = parameterInfo
        .GetCustomAttributes(typeof(InjectAttribute), true)
        .OfType<InjectAttribute>()
        .SingleOrDefault();

      return _predicate(injectAttribute);
    }

    private static Predicate<Attribute> CreateInjectAttributePredicate(object injectionPointId)
    {
      return attribute =>
      {
        var injectAttribute = attribute as InjectAttribute;
        return injectAttribute != null && Equals(injectAttribute.InjectionPointId, injectionPointId);
      };
    }

    
    
  }
}