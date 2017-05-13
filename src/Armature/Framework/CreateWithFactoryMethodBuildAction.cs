using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class CreateWithFactoryMethodBuildAction<TR> : IBuildAction
  {
    private readonly Func<Build.Session, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<Build.Session, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    public void Execute(Build.Session buildSession)
    {
      if(buildSession.BuildResult == null)
        buildSession.BuildResult = new BuildResult(_factoryMethod(buildSession));
    }

    public void PostProcess(Build.Session buildSession)
    {}
  }

  public abstract class CreateWithFactoryMethodBuildAction : IBuildAction
  {
    public void Execute(Build.Session buildSession)
    {
      if (buildSession.BuildResult == null)
      {
        var result = Execute(buildSession, buildSession.GetValuesForParameters(GetMethod().GetParameters()));
        buildSession.BuildResult = new BuildResult(result);
      }
    }

    public void PostProcess(Build.Session buildSession)
    {}

    protected abstract MethodBase GetMethod();
    protected abstract object Execute(Build.Session buildSession, object[] values);
  }

  public class CreateWithFactoryMethodBuildAction<T1,  TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<Build.Session, T1, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<Build.Session, T1, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(Build.Session buildSession, object[] values)
    {
      return _factoryMethod(buildSession, (T1) values[0]);
    }
  }

  public class CreateWithFactoryMethodBuildAction<T1, T2, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<Build.Session, T1, T2, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<Build.Session, T1, T2, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(Build.Session buildSession, object[] values)
    {
      return _factoryMethod(buildSession, (T1) values[0], (T2) values[1]);
    }
  }

  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<Build.Session, T1, T2, T3, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<Build.Session, T1, T2, T3, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(Build.Session buildSession, object[] values)
    {
      return _factoryMethod(buildSession, (T1) values[0], (T2) values[1], (T3) values[2]);
    }
  }
}