using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the property using <see cref="MemberInfo.Name"/> and specified key as an <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByPropertyName : BuildArgumentByInjectPointNameBase
{
  public BuildArgumentByPropertyName() { }
  public BuildArgumentByPropertyName(object? key) : base(key) { }

  protected override string GetInjectPointName(UnitId unitId) => ((PropertyInfo) unitId.Kind!).Name;
}