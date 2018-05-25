using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.BuildActions.Creation
{
  /// <summary>
  ///   Build action building an Unit using factory method with input parameters
  /// </summary>
  public abstract class CreateByFactoryMethodBuildAction : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if (!buildSession.BuildResult.HasValue)
      {
        var parameters = GetMethod().GetParameters().ToArray();
        var result = Execute(buildSession.GetValuesForParameters(parameters));
        buildSession.BuildResult = new BuildResult(result);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract MethodBase GetMethod();
    protected abstract object Execute(object[] values);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), GetMethod().ToLogString());
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) => _factoryMethod((T1)values[0]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) => _factoryMethod((T1)values[0], (T2)values[1]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, T3, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, T3, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) => _factoryMethod((T1)values[0], (T2)values[1], (T3)values[2]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, T4, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, T3, T4, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, T3, T4, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) =>
      _factoryMethod((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, T3, T4, T5, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, T3, T4, T5, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) =>
      _factoryMethod((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, T3, T4, T5, T6, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, T3, T4, T5, T6, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) =>
      _factoryMethod((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4], (T6)values[5]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction" />
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TR> factoryMethod) =>
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(object[] values) =>
      _factoryMethod((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4], (T6)values[5], (T7)values[6]);
  }
}