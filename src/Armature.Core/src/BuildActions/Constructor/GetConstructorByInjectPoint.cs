using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Gets the constructor of the type which is marked with <see cref="InjectAttribute" /> with the optional <see cref="InjectAttribute.Tag" />.
/// </summary>
public record GetConstructorByInjectPoint : IBuildAction, ILogString
{
  private readonly object? _injectPointTag;

  public GetConstructorByInjectPoint() { }
  public GetConstructorByInjectPoint(object? injectPointTag) => _injectPointTag = injectPointTag;

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();

    var constructors = unitType
                      .GetConstructors()
                      .Where(
                         ctor =>
                         {
                           var attributes = ctor.GetCustomAttributes<InjectAttribute>();
                           return attributes.Any(attribute => Equals(_injectPointTag, attribute.Tag));
                         })
                      .ToArray();

    if(constructors.Length > 1)
    {
      var exception = new ArmatureException(
        $"More than one constructors of the type {unitType.ToLogString()} are marked with attribute "
      + $"{nameof(InjectAttribute)} with {nameof(InjectAttribute.Tag)}={_injectPointTag.ToHoconString()}");

      for(var i = 0; i < constructors.Length; i++)
        exception.AddData($"Constructor #{i}", constructors[i]);

      throw exception;
    }

    var ctor = constructors.Length > 0 ? constructors[0] : null;
    ctor.WriteToLog(LogLevel.Trace);

    if(ctor is not null)
      buildSession.BuildResult = new BuildResult(constructors[0]);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetConstructorByInjectPoint)} {{ InjectPointId: {_injectPointTag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
