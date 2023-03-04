using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

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
    _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
  }

  public ISubjectTuner Building(Type type, object? tag = null) => Building(this, type, tag, Weight);

  public ISubjectTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => Treat(this, type, tag, Weight);

  public IBuildingTuner<T> Treat<T>(object? tag = null) => Treat<T>(this, tag, Weight);

  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null) => TreatOpenGeneric(this, openGenericType, tag, Weight);

  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag, Weight);

  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => TreatInheritorsOf<T>(this, tag, Weight);

  public IAllTuner TreatAll() => this;

  public IAllTuner UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  public IAllTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints) => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  public IAllTuner Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  ISubjectTuner ISubjectTuner.          AmendWeight(short delta) => AmendWeight(delta, this);
  IAllTuner IDependencyTuner<IAllTuner>.AmendWeight(short delta) => AmendWeight<IAllTuner>(delta, this);

  protected T AmendWeight<T>(short delta, T inheritor)
  {
    Weight += delta;
    return inheritor;
  }

  public ITuner?            Parent   { get; }
  public IBuildStackPattern TreeRoot { get; }
  public int                Weight   { get; private set; }

  public IBuildStackPattern GetOrAddNodeTo(IBuildStackPattern node) => node.GetOrAddNode(_createNode());

  #region Internals

  CreateNode IInternal<CreateNode>.Member1 => _createNode;

  #endregion
}
