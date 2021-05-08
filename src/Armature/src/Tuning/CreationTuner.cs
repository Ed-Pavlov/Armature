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
    public FinalTuner CreatedByDefault()
      => new(ParentNode.GetOrAddNode(new IfFirstUnit(new Pattern(Type, Key)).UseBuildAction(Default.CreationBuildAction, BuildStage.Create)));

    /// <summary>
    ///   Specifies that unit should be created using reflection.
    /// </summary>
    public FinalTuner CreatedByReflection()
      => new(ParentNode.GetOrAddNode(new IfFirstUnit(new Pattern(Type, Key)).UseBuildAction(CreateByReflection.Instance, BuildStage.Create)));
    
    Type IExtensibility<Type, object>.   Item1 => Type;
    object? IExtensibility<Type, object>.Item2 => Key;
  }
}
