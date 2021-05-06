using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Creates a Unit using specified factory method.
  /// </summary>
  public abstract class CreateWithFactoryMethodBuildAction : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(!buildSession.BuildResult.HasValue)
      {
        var method     = GetMethod();
        var arguments  = (object?[])buildSession.BuildUnit(new UnitId(method, SpecialKey.Argument)).Value!;
        var result     = Execute(arguments);
        buildSession.BuildResult = new BuildResult(result);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract MethodBase GetMethod();
    protected abstract object?    Execute(object?[] arguments);

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}( {GetMethod().ToLogString()} )";
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments) => _factoryMethod((T1?) arguments[0]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments) => _factoryMethod((T1?) arguments[0], (T2?) arguments[1]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, T3?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, T3?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments) => _factoryMethod((T1?) arguments[0], (T2?) arguments[1], (T3?) arguments[2]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, T3?, T4?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, T3?, T4?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments) => _factoryMethod((T1?) arguments[0], (T2?) arguments[1], (T3?) arguments[2], (T4?) arguments[3]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, T3?, T4?, T5?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, T3?, T4?, T5?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments)
      => _factoryMethod((T1?) arguments[0], (T2?) arguments[1], (T3?) arguments[2], (T4?) arguments[3], (T5?) arguments[4]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, T3?, T4?, T5?, T6?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, T3?, T4?, T5?, T6?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments)
      => _factoryMethod(
        (T1?) arguments[0],
        (T2?) arguments[1],
        (T3?) arguments[2],
        (T4?) arguments[3],
        (T5?) arguments[4],
        (T6?) arguments[5]);
  }

  /// <inheritdoc />
  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, TR?> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethodBuildAction(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, TR?> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object? Execute(object?[] arguments)
      => _factoryMethod(
        (T1?) arguments[0],
        (T2?) arguments[1],
        (T3?) arguments[2],
        (T4?) arguments[3],
        (T5?) arguments[4],
        (T6?) arguments[5],
        (T7?) arguments[6]);
  }
}
