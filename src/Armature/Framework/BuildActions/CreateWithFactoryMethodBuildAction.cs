using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  ///   Build action instantiates an object using passed factory method
  /// </summary>
  public class CreateWithFactoryMethodBuildAction<TR> : IBuildAction
  {
    private readonly Func<UnitBuilder, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

      _factoryMethod = factoryMethod;
    }

    public void Process(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult == null)
        unitBuilder.BuildResult = new BuildResult(_factoryMethod(unitBuilder));
    }

    [DebuggerStepThrough]
    public void PostProcess(UnitBuilder unitBuilder) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}: {1}", GetType().Name, _factoryMethod);
  }

  /// <summary>
  ///   Base class for build actions instantiates an object using factory method with input parameters
  /// </summary>
  public abstract class CreateWithFactoryMethodBuildAction : IBuildAction
  {
    public void Process(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult == null)
      {
        // remove UnitBuilder parameter from parameters array when resolving parameters values
        var result = Execute(unitBuilder, unitBuilder.GetValuesForParameters(GetMethod().GetParameters().Skip(1).ToArray()));
        unitBuilder.BuildResult = new BuildResult(result);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(UnitBuilder unitBuilder) { }

    protected abstract MethodBase GetMethod();
    protected abstract object Execute(UnitBuilder unitBuilder, object[] values);

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}: {1}", GetType().Name, GetMethod());
  }

  /// <summary>
  ///   Build action instantiates an object using passed factory method
  /// </summary>
  public class CreateWithFactoryMethodBuildAction<T1, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(UnitBuilder unitBuilder, object[] values) => _factoryMethod(unitBuilder, (T1)values[0]);
  }

  /// <summary>
  ///   Build action instantiates an object using passed factory method
  /// </summary>
  public class CreateWithFactoryMethodBuildAction<T1, T2, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, T2, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, T2, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(UnitBuilder unitBuilder, object[] values) => _factoryMethod(unitBuilder, (T1)values[0], (T2)values[1]);
  }

  /// <summary>
  ///   Build action instantiates an object using passed factory method
  /// </summary>
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, T2, T3, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, T2, T3, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(UnitBuilder unitBuilder, object[] values) => _factoryMethod(unitBuilder, (T1)values[0], (T2)values[1], (T3)values[2]);
  }
}