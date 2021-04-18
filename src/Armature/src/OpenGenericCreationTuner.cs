using System;
using Armature.Core;
using Armature.Extensibility;


namespace Armature
{
  public class OpenGenericCreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    OpenGenericType;
    protected readonly object? Key;

    public OpenGenericCreationTuner(IPatternTreeNode treeNode, Type openGenericType, object? key) : base(treeNode)
    {
      OpenGenericType = openGenericType;
      Key             = key;
    }

    Type IExtensibility<Type, object>.   Item1 => OpenGenericType;
    object? IExtensibility<Type, object>.Item2 => Key;

    public Tuner CreatedByDefault()
    {
      var childMatcher = new FindUnitMatches(
        new OpenGenericTypePattern(OpenGenericType, Key),
        QueryWeight.WildcardMatchingUnit - 1);

      ParentNode
       .GetOrAddNode(childMatcher)
       .UseBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(childMatcher);
    }

    public Tuner CreatedByReflection()
    {
      var childMatcher = new FindUnitMatches(
        new OpenGenericTypePattern(OpenGenericType, Key),
        QueryWeight.WildcardMatchingUnit - 1);

      ParentNode
       .GetOrAddNode(childMatcher)
       .UseBuildAction(BuildStage.Create, CreateByReflection.Instance);

      return new Tuner(childMatcher);
    }
  }
}
