﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Armature.Core;
using Armature.Core.Annotations;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature.BuildActions.Creation;

/// <summary>
/// Instantiates an object using reflection. See <see cref="ConstructorInfo.Invoke(object[])"/> method.
/// </summary>
public record CreateByReflection : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var type = buildSession.Stack.TargetUnit.GetUnitType();

    if(type is {IsInterface: false, IsAbstract: false})
    {
      var constructor = buildSession.GetConstructorOf(type);
      var parameters  = constructor.GetParameters();

      if(parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse business logic
        return;

      var arguments = parameters.Length == 0 ? Empty<object>.Array : buildSession.BuildArgumentsForMethod(parameters);
      try
      {
        var instance = constructor.Invoke(arguments);
        buildSession.BuildResult = new BuildResult(instance);
      }
      catch(TargetInvocationException exception)
      {
        if(exception.InnerException is null)
          throw;

        // throw "user" exception caused the TargetInvocationException w/o loosing the original stack trace
        ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        throw; // this call is for compiler which doesn't understand that capture.Throw() never returns
      }
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(CreateByReflection);
}