using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature
{
  public class OpenGenericCreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type OpenGenericType;
    protected readonly object Token;

    public OpenGenericCreationTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher, Type openGenericType, [CanBeNull] object token) : base(unitSequenceMatcher)
    {
      OpenGenericType = openGenericType;
      Token = token;
    }

    Type IExtensibility<Type, object>.Item1 => OpenGenericType;
    object IExtensibility<Type, object>.Item2 => Token;

    public Tuner CreatedByDefault()
    {
      var childMatcher = new WildcardUnitSequenceMatcher(new OpenGenericTypeMatcher(OpenGenericType, Token), UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(childMatcher)
        .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(childMatcher);
    }

    public Tuner CreatedByReflection()
    {
      var childMatcher = new WildcardUnitSequenceMatcher(new OpenGenericTypeMatcher(OpenGenericType, Token), UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(childMatcher)
        .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);

      return new Tuner(childMatcher);
    }
  }
}