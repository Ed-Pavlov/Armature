using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   "Builds" a property Unit of the currently building Unit marked with attribute which satisfies user provided conditions
  /// </summary>
  public class GetPropertyByAttributeBuildAction<T> : IBuildAction
    where T : Attribute
  {
    private readonly Predicate<T>? _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public GetPropertyByAttributeBuildAction(Predicate<T>? predicate = null) => _predicate = predicate;

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();

      var propertyList = type
        .GetProperties()
        .Where(
          property =>
            {
              var attribute = property.GetCustomAttribute<T>();
              return attribute != null && (_predicate == null || _predicate(attribute));
            })
        .ToArray();

      if (propertyList.Length > 0)
        buildSession.BuildResult = new BuildResult(propertyList);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _predicate);
  }
}