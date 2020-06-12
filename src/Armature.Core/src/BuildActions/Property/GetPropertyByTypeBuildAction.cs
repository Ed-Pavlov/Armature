using System;
using System.Linq;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   "Builds" a property Unit of the currently building Unit of specified type
  ///   specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public class GetPropertyByTypeBuildAction : IBuildAction
  {
    private readonly Type _type;

    public GetPropertyByTypeBuildAction([NotNull] Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();

      if (properties.Length == 0)
        throw new ArmatureException($"No property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}'");

      if (properties.Length > 1)
        throw new ArmatureException($"More than one property of type '{_type.ToLogString()}' in type '{unitType.ToLogString()}'");

      buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type);
  }
}