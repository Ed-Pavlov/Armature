using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Gets a constructor of type marked with <see cref="InjectAttribute" /> with the optional <see cref="InjectAttribute.InjectionPointId" />.
  /// </summary>
  public record GetConstructorByInjectPointId : IBuildAction
  {
    private readonly object? _injectPointId;
    
    public GetConstructorByInjectPointId(object? injectPointId = null) => _injectPointId = injectPointId;
    
    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();

      var constructors = unitType
                        .GetConstructors()
                        .Where(ctor =>
                               {
                                 var attribute = ctor.GetCustomAttribute<InjectAttribute>();
                                 return attribute is not null && Equals(_injectPointId, attribute.InjectionPointId);
                               })
                        .ToArray();

      if(constructors.Length > 1)
      {
        var exception = new ArmatureException(
          $"More than one constructors of the type {unitType.ToLogString()} are marked with attribute "
        + $"{nameof(InjectAttribute)} with {nameof(InjectAttribute.InjectionPointId)}={_injectPointId.ToLogString()}");

        for(var i = 0; i < constructors.Length; i++)
          exception.AddData($"Constructor #{i}", constructors[i].ToString());

        throw exception;
      }
      
      if(constructors.Length > 0)
        buildSession.BuildResult = new BuildResult(constructors[0]);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }
    
    public override string ToString() => GetType().GetShortName();
  }
}
