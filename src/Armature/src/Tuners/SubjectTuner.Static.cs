using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

public partial class SubjectTuner
{
  /// <inheritdoc cref="ISubjectTuner.Building"/>
  public static ISubjectTuner Building(ITuner parentTuner, Type type, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));

    var unitPattern = new UnitPattern(type, tag);
    return new SubjectTuner(parentTuner, CreateNode);

    IBuildStackPattern CreateNode() => new SkipTillUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern);
  }

  /// <inheritdoc cref="ISubjectTuner.Treat"/>
  public static IBuildingTuner<object?> Treat(ITuner parentTuner, Type type, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));
    if(type.IsGenericTypeDefinition) throw new ArgumentException($"Use {nameof(TreatOpenGeneric)} to setup open generic types.");

    var unitPattern = new UnitPattern(type, tag);
    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }

  /// <inheritdoc cref="ISubjectTuner.Treat{T}"/>
  public static IBuildingTuner<T> Treat<T>(ITuner parentTuner, object? tag, int weight = 0)
  {
    var unitPattern = new UnitPattern(typeof(T), tag);

    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }

  /// <inheritdoc cref="ISubjectTuner.TreatOpenGeneric"/>
  public static IBuildingTuner<object?> TreatOpenGeneric(ITuner parentTuner, Type openGenericType, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(openGenericType is null) throw new ArgumentNullException(nameof(openGenericType));

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);

    return new BuildingOpenGenericTuner(parentTuner, CreateNode, unitPattern);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.OpenGenericPattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }

  /// <inheritdoc cref="ISubjectTuner.TreatInheritorsOf"/>
  public static IBuildingTuner<object?> TreatInheritorsOf(ITuner parentTuner, Type baseType, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(baseType is null) throw new ArgumentNullException(nameof(baseType));

    var unitPattern = new IsInheritorOf(baseType, tag);
    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.SubtypePattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }

  /// <inheritdoc cref="ISubjectTuner.TreatInheritorsOf{T}"/>
  public static IBuildingTuner<T> TreatInheritorsOf<T>(ITuner parentTuner, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));

    var unitPattern = new IsInheritorOf(typeof(T), tag);
    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.SubtypePattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }
}
