using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class GetPropertyByAttributeBuildAction<T> : IBuildAction
    where T : Attribute
  {
    private readonly Predicate<T> _predicate;

    [DebuggerStepThrough]
    public GetPropertyByAttributeBuildAction(Predicate<T> predicate = null) => _predicate = predicate;

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();

      var propertyList = type
        .GetProperties()
        .Where(property => property.GetCustomAttribute<T>() != null)
        .ToArray();
      
      if(propertyList.Length > 0)
        buildSession.BuildResult = new BuildResult(propertyList);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _predicate);
  }
}