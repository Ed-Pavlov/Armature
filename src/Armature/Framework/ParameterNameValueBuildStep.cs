using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterNameValueBuildStep : ParameterValueBuildStep
  {
    private readonly string _parameterName;

    public ParameterNameValueBuildStep([NotNull] string parameterName, [NotNull] IBuildAction buildAction)
      : base(buildAction, ParameterValueBuildActionWeight.NamedParameterResolver)
    {
      if (parameterName == null) throw new ArgumentNullException("parameterName");
      _parameterName = parameterName;
    }

    protected override bool Matches(ParameterInfo parameterInfo)
    {
      var matches = _parameterName == parameterInfo.Name;

      if(!matches)
      {
        Log.Info("Does not match");
        Log.Info("MatchName={0}", _parameterName);
        Log.Info("ParameterName={0}", parameterInfo.Name);
      }

      return matches;
    }
  }
}