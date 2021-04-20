using System;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class OpenGenericCreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    OpenGenericType;
    protected readonly object? Key;

    public OpenGenericCreationTuner(IPatternTreeNode parentNode, Type openGenericType, object? key) : base(parentNode)
    {
      OpenGenericType = openGenericType ?? throw new ArgumentNullException(nameof(openGenericType));
      Key             = key;
    }

    /// <summary>
    ///   Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
    /// </summary>
    public Tuner CreatedByDefault()
      => new(ParentNode
            .GetOrAddNode(new FindUnitMatches(new OpenGenericTypePattern(OpenGenericType, Key), QueryWeight.WildcardMatchingUnit - 1))
            .UseBuildAction(Default.CreationBuildAction, BuildStage.Create));

    /// <summary>
    ///   Specifies that unit should be created using reflection.
    /// </summary>
    public Tuner CreatedByReflection() => 
      new(ParentNode
         .GetOrAddNode(new FindUnitMatches(new OpenGenericTypePattern(OpenGenericType, Key), QueryWeight.WildcardMatchingUnit - 1))
         .UseBuildAction(CreateByReflection.Instance, BuildStage.Create));

    Type IExtensibility<Type, object>.   Item1 => OpenGenericType;
    object? IExtensibility<Type, object>.Item2 => Key;
  }
}
