using System;
using Armature.Core;
using Armature.Core.Logging;

namespace Armature
{
  public class PropertyByType : LastUnitTuner, IInjectPointTuner
  {
    private readonly Type _type;

    public PropertyByType(Type type, int weight) : base(PropertiesListPattern.Instance, new GetPropertyByTypeBuildAction(type), weight)
      => _type = type ?? throw new ArgumentNullException(nameof(type));

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.ToLogString());
  }
}
