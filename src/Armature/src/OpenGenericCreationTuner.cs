using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitMatchers.UnitType;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;


namespace Armature
{
  public class OpenGenericCreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    OpenGenericType;
    protected readonly object? Key;

    public OpenGenericCreationTuner(IUnitSequenceMatcher unitSequenceMatcher, Type openGenericType, object? key) : base(unitSequenceMatcher)
    {
      OpenGenericType = openGenericType;
      Key           = key;
    }

    Type IExtensibility<Type, object>.   Item1 => OpenGenericType;
    object? IExtensibility<Type, object>.Item2 => Key;

    public Tuner CreatedByDefault()
    {
      var childMatcher = new WildcardUnitSequenceMatcher(
        new UnitKindIsOpenGenericTypeMatcher(OpenGenericType, Key),
        UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

      UnitSequenceMatcher
       .AddOrGetUnitSequenceMatcher(childMatcher)
       .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(childMatcher);
    }

    public Tuner CreatedByReflection()
    {
      var childMatcher = new WildcardUnitSequenceMatcher(
        new UnitKindIsOpenGenericTypeMatcher(OpenGenericType, Key),
        UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

      UnitSequenceMatcher
       .AddOrGetUnitSequenceMatcher(childMatcher)
       .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);

      return new Tuner(childMatcher);
    }
  }
}
