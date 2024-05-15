using System.Reflection;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Builds an argument for the property using <see cref="MemberInfo.Name"/> and specified tag as an <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByPropertyName : BuildArgumentByInjectPointNameBase
{
  public BuildArgumentByPropertyName() { }
  public BuildArgumentByPropertyName(object? tag) : base(tag) { }

  protected override string GetInjectPointName(UnitId unitId) => ((PropertyInfo) unitId.Kind!).Name;
}
