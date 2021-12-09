using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Instantiates an object using reflection <see cref="ConstructorInfo.Invoke(object[])"/> method.
/// </summary>
public record CreateByReflection : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var type = buildSession.GetUnitUnderConstruction().GetUnitType();
    Log.WriteLine(LogLevel.Verbose, () => $"Type = {type.ToLogString().QuoteIfNeeded()}");

    if(!type.IsInterface && !type.IsAbstract)
    {
      var constructor = buildSession.GetConstructorOf(type);
      var parameters  = constructor.GetParameters();

      if(parameters.Length == 0 && type.IsValueType) // do not create default value of value type, it can confuse logic
        return;

      object instance;
      if(parameters.Length == 0)
        instance = constructor.Invoke(Empty<object>.Array);
      else
      {
        var arguments = buildSession.BuildArgumentsForMethod(parameters);
        instance = constructor.Invoke(arguments);
      }

      buildSession.BuildResult = new BuildResult(instance);
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(CreateByReflection);
}