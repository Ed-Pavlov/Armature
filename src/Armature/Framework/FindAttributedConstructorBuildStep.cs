using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Finds a constructor marked with an <see cref="Attribute"/>. <see cref="InjectAttribute"/> is used by default
  /// by Armature framework. Provide your own <see cref=".ctor(Predicate{Attribute})"/> if you want to use another
  /// attribute
  /// </summary>
  public class FindAttributedConstructorBuildStep : FindConstructorBuildStepBase
  {
    private readonly Predicate<Attribute> _predicate;

    public FindAttributedConstructorBuildStep(int weight, object injectionPointId = null) : this(weight, CreateInjectAttributePredicate(injectionPointId))
    {}

    /// <summary>
    /// Use this constructor to use <see cref="FindAttributedConstructorBuildStep"/> with any other attribute rether then <see cref="InjectAttribute"/>
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public FindAttributedConstructorBuildStep(int weight, [NotNull] Predicate<Attribute> predicate) : base(weight)
    {
      if (predicate == null) throw new ArgumentNullException("predicate");
      _predicate = predicate;
    }

    [CanBeNull]
    protected override ConstructorInfo GetConstructor(Type type)
    {
      return type
        .GetConstructors()
        .SingleOrDefault(_ => 
            _.GetCustomAttributes(false)
            .OfType<Attribute>()
            .FirstOrDefault(attribute => _predicate(attribute)) 
          != null);
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