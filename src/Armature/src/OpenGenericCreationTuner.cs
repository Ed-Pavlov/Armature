using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Extensibility;


namespace Armature
{
  public class OpenGenericCreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    OpenGenericType;
    protected readonly object? Key;

    public OpenGenericCreationTuner(IQuery query, Type openGenericType, object? key) : base(query)
    {
      OpenGenericType = openGenericType;
      Key           = key;
    }

    Type IExtensibility<Type, object>.   Item1 => OpenGenericType;
    object? IExtensibility<Type, object>.Item2 => Key;

    public Tuner CreatedByDefault()
    {
      var childMatcher = new FindFirstUnit(
        new UnitKindIsOpenGenericTypeMatcher(OpenGenericType, Key), QueryWeight.WildcardMatchingUnit - 1);

      Query
       .AddSubQuery(childMatcher)
       .UseBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(childMatcher);
    }

    public Tuner CreatedByReflection()
    {
      var childMatcher = new FindFirstUnit(
        new UnitKindIsOpenGenericTypeMatcher(OpenGenericType, Key),
        QueryWeight.WildcardMatchingUnit - 1);

      Query
       .AddSubQuery(childMatcher)
       .UseBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);

      return new Tuner(childMatcher);
    }
  }
}
