using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.Name"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public record BuildArgumentByParameterName : BuildArgumentByInjectPointNameBase
  {
    public BuildArgumentByParameterName() { }
    public BuildArgumentByParameterName(object? key) : base(key) { }

    protected override string GetInjectPointName(UnitId unitId) => ((ParameterInfo) unitId.Kind!).Name;
    
    public override string ToString() => base.ToString();
  }
}
