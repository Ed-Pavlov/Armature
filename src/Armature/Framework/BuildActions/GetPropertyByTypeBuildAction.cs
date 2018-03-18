using System;
using System.Linq;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.BuildActions
{
  public class GetPropertyByTypeBuildAction : IBuildAction
  {
    private readonly Type _type;

    public GetPropertyByTypeBuildAction([NotNull] Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();
      var properties = type.GetProperties().Where(_ => _.PropertyType == _type).ToArray();
      if(properties.Length > 0)
        buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type);
  }
}