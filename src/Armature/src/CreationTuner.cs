using System;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;

namespace Armature
{
  public class CreationTuner : UnitSequenceExtensibility, IExtensibility<Type, object>
  {
    protected readonly Type    Type;
    protected readonly object? Key;

    public CreationTuner(IUnitSequenceMatcher unitSequenceMatcher, Type type, object? key) : base(unitSequenceMatcher)
    {
      Type  = type ?? throw new ArgumentNullException(nameof(type));
      Key = key;
    }

    Type IExtensibility<Type, object>.   Item1 => Type;
    object? IExtensibility<Type, object>.Item2 => Key;

    /// <summary>
    ///   Specifies that unit of type passed into <see cref="TreatingTuner{T}.As(System.Type,object)"/> or <see cref="TreatingTuner{T}.As{TRedirect}"/>
    ///   should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
    /// </summary>
    public Tuner CreatedByDefault()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(new UnitInfoMatcher(Type, Key));

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
      var sequenceMatcher = new StrictUnitSequenceMatcher(new UnitInfoMatcher(Type, Key));

      UnitSequenceMatcher
       .AddOrGetUnitSequenceMatcher(sequenceMatcher)
       .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);

      return new Tuner(sequenceMatcher);
    }
  }
}
