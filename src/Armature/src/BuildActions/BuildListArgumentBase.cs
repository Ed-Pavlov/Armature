using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Armature.Core;
using Armature.Core.Annotations;

namespace Armature;

/// <summary>
/// Base class for build actions build a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method.
/// </summary>
public abstract record BuildListArgumentBase : IBuildAction, ILogString
{
  private static readonly object[] ParamContainer     = new object[1];
  private static readonly Type[]   TypeParamContainer = new Type[1];
  private static readonly Type[]   IntTypeParam       = {typeof(int)};

  private readonly object? _tag;

  [WithoutTest]
  [DebuggerStepThrough]
  protected BuildListArgumentBase() { }
  [DebuggerStepThrough]
  protected BuildListArgumentBase(object? tag) => _tag = tag;

  public void Process(IBuildSession buildSession)
  {
    if(Log.IsEnabled(LogLevel.Trace))
      Log.WriteLine(LogLevel.Trace, $"Tag: {_tag.ToHoconString()}");

    var targetUnit   = buildSession.Stack.TargetUnit;
    var effectiveTag = _tag.GetEffectiveTag(targetUnit.Tag);

    var injectionPointType = GetArgumentType(targetUnit);

    if(IsCollection(injectionPointType, out var listType))
    {
      var collectionItemType = injectionPointType.GenericTypeArguments[0];
      var arguments          = buildSession.BuildAllUnits(Unit.Of(collectionItemType, effectiveTag));

      var listInstance = CreateListInstance(listType!, arguments.Count);
      FillList(listInstance, listType!, collectionItemType, arguments.Select(_ => _.Entity));

      buildSession.BuildResult = new BuildResult(listInstance);
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void PostProcess(IBuildSession buildSession) { }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected abstract Type GetArgumentType(UnitId unitId);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static object CreateListInstance(Type listType, int capacity)
  {
    var listConstructor = listType.GetConstructor(IntTypeParam);

    return listConstructor!.Invoke(CreateParameter(capacity));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void FillList(object listInstance, Type listType, Type itemType, IEnumerable<BuildResult> values)
  {
    var addMethod = listType.GetMethod(nameof(List<object>.Add), CreateTypeParameter(itemType));

    foreach(var buildResult in values)
      addMethod!.Invoke(listInstance, CreateParameter(Convert.ChangeType(buildResult.Value, itemType)));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static object[] CreateParameter(object value)
  {
    ParamContainer[0] = value;

    return ParamContainer;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static Type[] CreateTypeParameter(Type type)
  {
    TypeParamContainer[0] = type;

    return TypeParamContainer;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsCollection(
    Type type,
    // [NotNullWhen(true)]
    out Type? listType)
  {
    listType = null;

    if(!type.IsGenericType) return false;
    if(type.GenericTypeArguments.Length != 1) return false;

    listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);

    return type.IsAssignableFrom(listType);
  }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}