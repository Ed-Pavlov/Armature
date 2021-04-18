using System;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class CreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    Type;
    protected readonly object? Key;

    public CreationTuner(IPatternTreeNode parentNode, Type type, object? key) : base(parentNode)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Key  = key;
    }

    /// <summary>
    ///   Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
    /// </summary>
    public Tuner CreatedByDefault()
      => new(ParentNode.GetOrAddNode(new IfFirstUnitMatches(new Pattern(Type, Key)).UseBuildAction(BuildStage.Create, Default.CreationBuildAction)));

    /// <summary>
    ///   Specifies that unit should be created using reflection.
    /// </summary>
    public Tuner CreatedByReflection()
      => new(ParentNode.GetOrAddNode(new IfFirstUnitMatches(new Pattern(Type, Key)).UseBuildAction(BuildStage.Create, CreateByReflection.Instance)));
    
    Type IExtensibility<Type, object>.   Item1 => Type;
    object? IExtensibility<Type, object>.Item2 => Key;
  }
}
