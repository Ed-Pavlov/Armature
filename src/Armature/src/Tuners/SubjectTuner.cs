using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

public partial class SubjectTuner : ISubjectTuner, IAllTuner, ITuner, IInternal<CreateNode>
{
  [PublicAPI]
  protected readonly CreateNode _createNode;

  [DebuggerStepThrough]
  [PublicAPI]
  public SubjectTuner(ITuner parent, CreateNode createNode)
  {
    Parent      = parent;
    TreeRoot    = parent.TreeRoot;
    Weight      = parent.Weight;
    _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
  }

  public ISubjectTuner Building(Type type, object? tag = null) => Building(this, type, tag);

  public ISubjectTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => Treat(this, type, tag);

  public IBuildingTuner<T> Treat<T>(object? tag = null) => Treat<T>(this, tag);

  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null) => TreatOpenGeneric(this, openGenericType, tag);

  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag);

  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => TreatInheritorsOf<T>(this, tag);

  public IAllTuner TreatAll() => this;

  public IAllTuner UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  public IAllTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints) => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  public IAllTuner Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  ISubjectTuner ISubjectTuner.          AmendWeight(int delta) => AmendWeight(delta, this);
  IAllTuner IDependencyTuner<IAllTuner>.AmendWeight(int delta) => AmendWeight<IAllTuner>(delta, this);

  protected T AmendWeight<T>(int delta, T inheritor)
  {
    Weight += delta;
    return inheritor;
  }

  public ITuner?            Parent   { get; }
  public IBuildStackPattern TreeRoot { get; }
  public int                Weight   { get; private set; }

  public IBuildStackPattern GetOrAddNodeTo(IBuildStackPattern node) => node.GetOrAddNode(_createNode(Weight));

  #region Internals

  CreateNode IInternal<CreateNode>.Member1 => _createNode;

  #endregion
}
