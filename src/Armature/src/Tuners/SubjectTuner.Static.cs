using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class SubjectTuner
{
  public static ISubjectTuner Building(ITuner parentTuner, Type type, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));

    var unitPattern = new UnitPattern(type, tag);
    IBuildStackPattern CreateNode() => new SkipTillUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.SkipTillUnit);
    return new SubjectTuner(parentTuner, CreateNode);
  }

  public static IBuildingTuner<object?> Treat(ITuner parentTuner, Type type, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(type is null) throw new ArgumentNullException(nameof(type));
    if(type.IsGenericTypeDefinition) throw new ArgumentException($"Use {nameof(TreatOpenGeneric)} to setup open generic types.");

    var unitPattern = new UnitPattern(type, tag);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.IfFirstUnit);

    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<T> Treat<T>(ITuner parentTuner, object? tag, int weight = 0)
  {
    var unitPattern = new UnitPattern(typeof(T), tag);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<object?> TreatOpenGeneric(ITuner parentTuner, Type openGenericType, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(openGenericType is null) throw new ArgumentNullException(nameof(openGenericType));

    var unitPattern = new IsGenericOfDefinition(openGenericType, tag);

    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.OpenGenericPattern + WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingOpenGenericTuner(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<object?> TreatInheritorsOf(ITuner parentTuner, Type baseType, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));
    if(baseType is null) throw new ArgumentNullException(nameof(baseType));

    var unitPattern = new IsInheritorOf(baseType, tag);
    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.SubtypePattern + WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingTuner<object?>(parentTuner, CreateNode, unitPattern);
  }

  public static IBuildingTuner<T> TreatInheritorsOf<T>(ITuner parentTuner, object? tag, int weight = 0)
  {
    if(parentTuner is null) throw new ArgumentNullException(nameof(parentTuner));

    var unitPattern = new IsInheritorOf(typeof(T), tag);
    IBuildStackPattern CreateNode() => new IfFirstUnit(unitPattern, weight + WeightOf.UnitPattern.SubtypePattern + WeightOf.BuildStackPattern.IfFirstUnit);
    return new BuildingTuner<T>(parentTuner, CreateNode, unitPattern);
  }
}