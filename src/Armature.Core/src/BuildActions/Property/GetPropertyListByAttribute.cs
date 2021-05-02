using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Gets a list of  properties marked with attribute which satisfies user provided conditions.
  /// </summary>
  public class GetPropertyListByAttribute<T> : IBuildAction
    where T : Attribute
  {
    private readonly Predicate<T>? _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public GetPropertyListByAttribute(Predicate<T>? predicate = null) => _predicate = predicate;

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();

      var propertyList = type
                        .GetProperties()
                        .Where(
                           property =>
                           {
                             var attribute = property.GetCustomAttribute<T>();

                             return attribute is not null && (_predicate is null || _predicate(attribute));
                           })
                        .ToArray();

      if(propertyList.Length > 0)
        buildSession.BuildResult = new BuildResult(propertyList);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}( {1} )", GetType().GetShortName(), _predicate);
  }
}
