using System;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner
{
  public static IBuildingTuner Building(ITunerInternal parentTuner, Type type, object? tag, short weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));

    var unitPattern = new UnitPattern(type, tag);
    IBuildChainPattern CreateNode() => new SkipTillUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.SkipTillUnit);
    return new BuildingTuner(parentTuner, CreateNode, weight);
  }

  public static IBuildingTuner<object?> Treat(ITunerInternal parentTuner, Type type, object? tag, short weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));
    if(type.IsGenericTypeDefinition) throw new ArgumentException($"Use {nameof(TreatOpenGeneric)} to setup open generic types.");

    var unitPattern = new UnitPattern(type, tag);

    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.TargetUnit);

    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<T> Treat<T>(ITunerInternal parentTuner, object? tag, short weight = 0)
  {
    var unitPattern = new UnitPattern(typeof(T), tag);

    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.TargetUnit);
    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<object?> TreatOpenGeneric(ITunerInternal parentTuner, Type openGenericType, object? tag, short weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(openGenericType is null) throw new ArgumentNullException(nameof(openGenericType));

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);

    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.OpenGenericPattern + WeightOf.BuildChainPattern.TargetUnit);
    return new BuildingOpenGenericTuner(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<object?> TreatInheritorsOf(ITunerInternal parentTuner, Type baseType, object? tag, short weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(baseType is null) throw new ArgumentNullException(nameof(baseType));

    var baseWeight  = weight + WeightOf.UnitPattern.SubtypePattern;
    var unitPattern = new IsInheritorOf(baseType, tag);
    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit);
    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<T> TreatInheritorsOf<T>(ITunerInternal parentTuner, object? tag, short weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));

    var baseWeight  = weight + WeightOf.UnitPattern.SubtypePattern;
    var unitPattern = new IsInheritorOf(typeof(T), tag);
    IBuildChainPattern CreateNode() => new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit);
    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);
  }
}