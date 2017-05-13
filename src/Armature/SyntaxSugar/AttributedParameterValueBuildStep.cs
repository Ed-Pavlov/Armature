using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature
{
  /// <summary>
  /// Contains a build step to build a value for a parameter marked with an <see cref="Attribute"/>.
  /// <see cref="InjectAttribute"/> is used by default  by Armature framework.
  /// Provide your own <see cref=".ctor(Predicate{Attribute})"/> if you want to use another attribute
  /// </summary>
  public class AttributedParameterValueBuildStep : ParameterValueBuildStep
  {
    private readonly Predicate<Attribute> _predicate;

    public AttributedParameterValueBuildStep([CanBeNull] object injectPointId, [NotNull] IBuildAction buildAction)
      : this(buildAction, CreateInjectAttributePredicate(injectPointId))
    {}

    /// <summary>
    /// Use this constructor to use <see cref="AttributedParameterValueBuildStep"/> with any other attribute rether then <see cref="InjectAttribute"/>
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public AttributedParameterValueBuildStep([NotNull] IBuildAction buildAction, [NotNull] Predicate<Attribute> predicate)
      : base(buildAction, ParameterValueBuildActionWeight.AttributedParameterResolver)
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

      var matches = _predicate(injectAttribute);

      if(!matches)
      {
        Log.Info("Does not match");
//        Log.Info("MatchId={0}", _injectPointId ?? "null");
        Log.Info("ParameterId={0}", injectAttribute == null ? "not marked" : injectAttribute.InjectionPointId ?? "null");
      }

      return matches;
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