using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Creates a Unit using specified factory method.
  /// </summary>
  public class CreateWithFactoryMethod<TR> : IBuildAction
  {
    private readonly Func<IBuildSession, TR> _factoryMethod;

    [DebuggerStepThrough]
    public CreateWithFactoryMethod(Func<IBuildSession, TR> factoryMethod)
      => _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

    public void Process(IBuildSession buildSession)
    {
      if(!buildSession.BuildResult.HasValue)
        buildSession.BuildResult = new BuildResult(_factoryMethod(buildSession));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _factoryMethod.ToLogString());
  }
}
