using System;
using System.Linq;
using Armature.Core;
using Armature.Logging;
using Armature.Properties;

namespace Armature.Framework.BuildActions.Property
{
  public class GetPropertyByTypeBuildAction : IBuildAction
  {
    private readonly Type _type;

    public GetPropertyByTypeBuildAction([NotNull] Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var properties = unitType.GetProperties().Where(_ => _.PropertyType == _type).ToArray();
      
      if(properties.Length == 0)
        throw new ArmatureException($"No property of type {_type.AsLogString()} in type {unitType.AsLogString()}");
      
      if(properties.Length > 1)
        throw new ArmatureException($"Ambiguity: there are more that one property of type {_type.AsLogString()} in type {unitType.AsLogString()}");
      
      buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type);
  }
}