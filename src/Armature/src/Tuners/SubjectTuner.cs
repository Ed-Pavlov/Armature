using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

// ReSharper disable ArrangeStaticMemberQualifier: despite this class is not static, almost all this logic is delegated to static methods, it's important to see this

namespace BeatyBit.Armature;

public partial class SubjectTuner : ISubjectTuner, IAllTuner, ITuner, IInternal<CreateNode>
{
  protected readonly ITuner?            _parent;
  protected readonly IBuildStackPattern _treeRoot;
  protected readonly CreateNode         _createNode;

  private int _weight;

  [DebuggerStepThrough]
  [PublicAPI]
  public SubjectTuner(ITuner parent, CreateNode createNode)
  {
    _parent     = parent;
    _treeRoot   = parent.TreeRoot;
    _weight     = parent.Weight;
    _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
  }


  public ISubjectTuner Building(Type type, object? tag = null) => SubjectTuner.Building(this, type, tag);

  public ISubjectTuner Building<T>(object? tag = null) => Building(typeof(T), tag);

  public IBuildingTuner<object?> Treat(Type type, object? tag = null) => SubjectTuner.Treat(this, type, tag);

  public IBuildingTuner<T> Treat<T>(object? tag = null) => SubjectTuner.Treat<T>(this, tag);

  public IBuildingTuner<object?> TreatOpenGeneric(Type openGenericType, object? tag = null) => SubjectTuner.TreatOpenGeneric(this, openGenericType, tag);

  public IBuildingTuner<object?> TreatInheritorsOf(Type baseType, object? tag = null) => TreatInheritorsOf(this, baseType, tag);

  public IBuildingTuner<T> TreatInheritorsOf<T>(object? tag = null) => SubjectTuner.TreatInheritorsOf<T>(this, tag);

  public IAllTuner TreatAll() => this;

  public IAllTuner UsingArguments(params object[] arguments) => DependencyTuner.UsingArguments(this, arguments);

  public IAllTuner UsingInjectionPoints(params IInjectionPointSideTuner[] injectionPoints) => DependencyTuner.UsingInjectionPoints(this, injectionPoints);

  public IAllTuner Using(params ISideTuner[] sideTuners) => DependencyTuner.Using(this, sideTuners);

  ISubjectTuner ISubjectTuner.          AmendWeight(int delta) => AmendWeight(delta, this);
  IAllTuner IDependencyTuner<IAllTuner>.AmendWeight(int delta) => AmendWeight<IAllTuner>(delta, this);

  protected T AmendWeight<T>(int delta, T inheritor)
  {
    _weight += delta;
    return inheritor;
  }


  ITuner? ITuner.Parent => _parent;

  IBuildStackPattern ITuner.TreeRoot => _treeRoot;


  int ITuner.Weight => _weight;

  IBuildStackPattern ITuner.GetOrAddNodeTo(IBuildStackPattern node) => node.GetOrAddNode(_createNode(_weight));

  #region Internals

  CreateNode IInternal<CreateNode>.Member1 => _createNode;

  #endregion
}
