using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature;

public class CreationTuner : TunerBase, IInternal<Type, object?>
{
  [PublicAPI]
  protected readonly Type Type;
  [PublicAPI]
  protected readonly object? Tag;

  public CreationTuner(IBuildChainPattern parentNode, Type type, object? tag) : base(parentNode)
  {
    Type = type ?? throw new ArgumentNullException(nameof(type));
    Tag  = tag;
  }

  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  public FinalTuner CreatedByDefault()
    => new(
      ParentNode.GetOrAddNode(
        new IfFirstUnit(new UnitPattern(Type, Tag))
         .UseBuildAction(Default.CreationBuildAction, BuildStage.Create)));

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  public FinalTuner CreatedByReflection()
    => new(
      ParentNode.GetOrAddNode(
        new IfFirstUnit(new UnitPattern(Type, Tag))
         .UseBuildAction(Static.Of<CreateByReflection>(), BuildStage.Create)));

  Type IInternal<Type>.            Member1 => Type;
  object? IInternal<Type, object?>.Member2 => Tag;
}
