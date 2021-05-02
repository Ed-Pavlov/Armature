using System;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Gets a list of  properties marked with <see cref="InjectAttribute" /> with the optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public class GetPropertyListByInjectPointId : IBuildAction
  {
    private readonly object?[] _pointIds;

    public GetPropertyListByInjectPointId(params object?[] pointIds) => _pointIds = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();

      var propertiesWithAttributes =
        type.GetProperties()
            .Select(
               property =>
               {
                 var attribute = property.GetCustomAttribute<InjectAttribute>();
                 return Tuple.Create(attribute, property);
               })
            .Where(_ => _.Item1 is not null)
            .ToArray();

      var properties =
        (_pointIds.Length > 0
           ? _pointIds.SelectMany(pointId => propertiesWithAttributes.Where(_ => Equals(pointId, _.Item1.InjectionPointId)).Select(_ => _.Item2))
                      .Distinct()
           : propertiesWithAttributes.Select(_ => _.Item2))
       .ToArray();

      if(properties.Length > 0)
        buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format("{0}( {1} )", GetType().GetShortName(), string.Join(", ", _pointIds));
  }
}
