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

    public OpenGenericCreationTuner(IPatternTreeNode patternTreeNode, Type openGenericType, object? key) : base(patternTreeNode)
    {
      OpenGenericType = openGenericType;
      Key             = key;
    }

    Type IExtensibility<Type, object>.   Item1 => OpenGenericType;
    object? IExtensibility<Type, object>.Item2 => Key;

    public Tuner CreatedByDefault()
    {
      var childMatcher = new FindUnitMatches(
        new IsOpenGenericTypePattern(OpenGenericType, Key),
        QueryWeight.WildcardMatchingUnit - 1);

      PatternTreeNode
       .GetOrAddNode(childMatcher)
       .UseBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(childMatcher);
    }

    public Tuner CreatedByReflection()
    {
      var childMatcher = new FindUnitMatches(
        new IsOpenGenericTypePattern(OpenGenericType, Key),
        QueryWeight.WildcardMatchingUnit - 1);

      PatternTreeNode
       .GetOrAddNode(childMatcher)
       .UseBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);

      return new Tuner(childMatcher);
    }
  }
}
