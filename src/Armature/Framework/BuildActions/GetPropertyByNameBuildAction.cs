using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.BuildActions
{
  public class GetPropertyByNameBuildAction : IBuildAction
  {
    private readonly IReadOnlyCollection<string> _names;

    public GetPropertyByNameBuildAction([NotNull] params string[] names)
    {
      if (names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));
      _names = names;
    }

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();
      
      var propertiesList = 
      _names.Select(
          name =>
            {
              var property = type.GetProperty(name);
              if (property == null)
                throw new ArmatureException(string.Format("There is no property {0} in type {1}", _names, type.AsLogString()));

              return property;
            })
        .ToArray();
      
      buildSession.BuildResult = new BuildResult(propertiesList);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}