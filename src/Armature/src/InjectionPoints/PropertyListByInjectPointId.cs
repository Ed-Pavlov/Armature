using System.Linq;
using Armature.Core;
using Armature.Core.Logging;

namespace Armature
{
  /// <summary>
  ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding point ids
  /// </summary>
  public class PropertyListByInjectPointId : LastUnitTuner, IInjectPointTuner //, IExtensibility<object[]>
  {
    private readonly object[] _pointIds;

    public PropertyListByInjectPointId(object[] pointIds, int weight) : base(PropertiesListPattern.Instance, new GetPropertyListByInjectPointId(pointIds), weight) 
      => _pointIds = pointIds;

    public override string ToString()
      => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _pointIds.Select(_ => _.ToLogString())));
  }
}
