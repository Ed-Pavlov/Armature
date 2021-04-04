using System;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;


namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   "Builds" a list of properties Unit of the currently building Unit marked with <see cref="InjectAttribute" /> with
  ///   specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public class GetPropertyByInjectPointBuildAction : IBuildAction
  {
    private readonly object?[] _pointIds;

    public GetPropertyByInjectPointBuildAction(params object?[] pointIds) => _pointIds = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public void Process(IBuildSession buildSession)
    {
      var type = buildSession.GetUnitUnderConstruction().GetUnitType();

      var propertiesWithAttributes = type.GetProperties()
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
          ? _pointIds.SelectMany(pointId => propertiesWithAttributes.Where(_ => Equals(pointId, _.Item1.InjectionPointId)).Select(_ => _.Item2)).Distinct()
          : propertiesWithAttributes.Select(_ => _.Item2))
        .ToArray();

      if (properties.Length > 0)
        buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _pointIds));
  }
}