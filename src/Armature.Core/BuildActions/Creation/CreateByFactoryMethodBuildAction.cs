﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Resharper.Annotations;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Creation
{
  /// <summary>
  ///   Build an Unit using passed factory method
  /// </summary>
  public class CreateByFactoryMethodBuildAction<TR> : IBuildAction
  {
    private readonly Func<IBuildSession, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<IBuildSession, TR> factoryMethod) => 
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    public void Process(IBuildSession buildSession)
    {
      if (buildSession.BuildResult == null)
        buildSession.BuildResult = new BuildResult(_factoryMethod(buildSession));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _factoryMethod.AsLogString());
  }

  /// <summary>
  ///   Base class for build actions building an Unit using factory method with input parameters
  /// </summary>
  public abstract class CreateByFactoryMethodBuildAction : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if (buildSession.BuildResult == null)
      {
        // remove BuildSession parameter from parameters array when resolving parameters values
        var result = Execute(buildSession, buildSession.GetValuesForParameters(GetMethod().GetParameters().Skip(1).ToArray()));
        buildSession.BuildResult = new BuildResult(result);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract MethodBase GetMethod();
    protected abstract object Execute(IBuildSession buildSessoin, object[] values);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), GetMethod().AsLogString());
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction"/>
  public class CreateByFactoryMethodBuildAction<T1, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<IBuildSession, T1, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<IBuildSession, T1, TR> factoryMethod) => 
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(IBuildSession buildSessoin, object[] values) => _factoryMethod(buildSessoin, (T1)values[0]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction"/>
  public class CreateByFactoryMethodBuildAction<T1, T2, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<IBuildSession, T1, T2, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<IBuildSession, T1, T2, TR> factoryMethod) => 
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(IBuildSession buildSessoin, object[] values) => _factoryMethod(buildSessoin, (T1)values[0], (T2)values[1]);
  }

  /// <inheritdoc cref="CreateByFactoryMethodBuildAction"/>
  public class CreateByFactoryMethodBuildAction<T1, T2, T3, TR> : CreateByFactoryMethodBuildAction
  {
    private readonly Func<IBuildSession, T1, T2, T3, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateByFactoryMethodBuildAction([NotNull] Func<IBuildSession, T1, T2, T3, TR> factoryMethod) => 
      _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    protected override MethodBase GetMethod() => _factoryMethod.Method;

    protected override object Execute(IBuildSession buildSessoin, object[] values) => _factoryMethod(buildSessoin, (T1)values[0], (T2)values[1], (T3)values[2]);
  }
}