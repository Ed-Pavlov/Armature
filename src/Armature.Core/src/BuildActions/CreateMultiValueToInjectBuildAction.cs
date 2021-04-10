using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Armature.Core.Logging;


namespace Armature.Core.BuildActions
{
  public abstract class CreateMultiValueToInjectBuildAction : IBuildAction
  {
    private static readonly object[] ParamContainer     = new object[1];
    private static readonly Type[]   TypeParamContainer = new Type[1];
    private static readonly Type[]   IntTypeParam       = {typeof(int)};

    private readonly object? _key;

    [DebuggerStepThrough]
    protected CreateMultiValueToInjectBuildAction(object? key) => _key = key;

    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();
      var effectiveKey        = _key == UnitKey.Propagate ? unitUnderConstruction.Key : _key;

      var valueType = GetValueType(unitUnderConstruction);

      if(IsCollection(valueType, out var listType))
      {
        var itemType = valueType.GenericTypeArguments[0];

        var values = buildSession.BuildAllUnits(new UnitId(itemType, effectiveKey));

        if(values is not null)
        {
          if(listType is null)
            throw new InvalidOperationException("Remove this assert when support of .NET4.x will be discarded and use Roslyn analyzer attributes");

          var listInstance = CreateListInstance(listType, values.Count);
          FillList(listInstance, listType, itemType, values);

          buildSession.BuildResult = new BuildResult(listInstance);
        }
      }
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PostProcess(IBuildSession buildSession) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract Type GetValueType(UnitId unitId);

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
    private static bool IsCollection(Type type, out Type? listType)
    {
      listType = null;

      if(!type.IsGenericType) return false;
      if(type.GenericTypeArguments.Length != 1) return false;

      listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);

      return type.IsAssignableFrom(listType);
    }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _key.ToLogString());
  }
}
