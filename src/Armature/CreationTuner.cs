using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature
{
  public class CreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type Type;
    protected readonly object Token;

    public CreationTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [NotNull] Type type, [CanBeNull] object token) : base(unitSequenceMatcher)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Token = token;
    }

    Type IExtensibility<Type, object>.Item1 => Type;
    object IExtensibility<Type, object>.Item2 => Token;

    /// <summary>
    ///   Specifies that unit of type passed into <see cref="TreatingTuner{T}.As(System.Type,object)"/> or <see cref="TreatingTuner{T}.As{TRedirect}"/>
    ///   should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
    /// </summary>
    public Tuner CreatedByDefault()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type(Type, Token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(sequenceMatcher);
    }

    /// <summary>
    ///   Specifies that unit of type passed into <see cref="TreatingTuner{T}.As(System.Type,object)"/> or <see cref="TreatingTuner{T}.As{TRedirect}"/> should
    ///   be created using reflection
    /// </summary>
    /// <returns></returns>
    public Tuner CreatedByReflection()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type(Type, Token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(sequenceMatcher);
    }
  }
}