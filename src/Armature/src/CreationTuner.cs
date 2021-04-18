using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Extensibility;

namespace Armature
{
  public class CreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    Type;
    protected readonly object? Key;

    public CreationTuner(IPatternTreeNode patternTreeNode, Type type, object? key) : base(patternTreeNode)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Key  = key;
    }

    Type IExtensibility<Type, object>.   Item1 => Type;
    object? IExtensibility<Type, object>.Item2 => Key;

    /// <summary>
    ///   Specifies that unit of type passed into <see cref="TreatingTuner{T}.As(System.Type,object)"/> or <see cref="TreatingTuner{T}.As{TRedirect}"/>
    ///   should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
    /// </summary>
    public Tuner CreatedByDefault()
      => new(PatternTreeNode.GetOrAddNode(
               new IfFirstUnitMatches(new UnitIdPattern(Type, Key))
                .UseBuildAction(BuildStage.Create, Default.CreationBuildAction)
             )
      );

    /// <summary>
    ///   Specifies that unit of type passed into <see cref="TreatingTuner{T}.As(System.Type,object)"/> or <see cref="TreatingTuner{T}.As{TRedirect}"/> should
    ///   be created using reflection
    /// </summary>
    /// <returns></returns>
    public Tuner CreatedByReflection()
      => new(PatternTreeNode.GetOrAddNode(
               new IfFirstUnitMatches(new UnitIdPattern(Type, Key))
                .UseBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance)
             )
      );
  }
}
