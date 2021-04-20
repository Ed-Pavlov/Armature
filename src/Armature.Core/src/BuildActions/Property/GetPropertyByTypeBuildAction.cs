using System;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   "Builds" a property Unit of the currently building Unit of specified type
  ///   specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public class GetPropertyByTypeBuildAction : IBuildAction
  {
    private readonly Type _type;

    public GetPropertyByTypeBuildAction(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public void Process(IBuildSession buildSession)
    {
      var unitType   = buildSession.GetUnitUnderConstruction().GetUnitType();
      var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();

      buildSession.BuildResult =
        properties.Length switch
        {
          0   => throw new ArmatureException($"No property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}'"),
          > 1 => throw new ArmatureException($"More than one property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}'"),
          _   => new BuildResult(properties)
        };
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type);
  }
}
