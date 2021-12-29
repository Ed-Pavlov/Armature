using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature;

public class OpenGenericCreationTuner : TunerBase, IInternal<Type, object?>
{
  [PublicAPI]
  protected readonly Type OpenGenericType;
  [PublicAPI]
  protected readonly object? Tag;

  public OpenGenericCreationTuner(IBuildChainPattern parentNode, Type openGenericType, object? tag) : base(parentNode)
  {
    OpenGenericType = openGenericType ?? throw new ArgumentNullException(nameof(openGenericType));
    Tag             = tag;
  }

  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  public FinalTuner CreatedByDefault()
    => new(
      ParentNode
       .GetOrAddNode(new IfFirstUnit(new IsGenericOfDefinition(OpenGenericType, Tag)))
       .UseBuildAction(Default.CreationBuildAction, BuildStage.Create));

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  public FinalTuner CreatedByReflection()
    => new(
      ParentNode
       .GetOrAddNode(new SkipTillUnit(new IsGenericOfDefinition(OpenGenericType, Tag)))
       .UseBuildAction(Static.Of<CreateByReflection>(), BuildStage.Create));

  Type IInternal<Type>.            Member1 => OpenGenericType;
  object? IInternal<Type, object?>.Member2 => Tag;
}
