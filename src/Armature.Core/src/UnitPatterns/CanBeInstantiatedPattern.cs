using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if <see cref="UnitId.Kind"/> is a type which can be instantiated.
  /// </summary>
  public record CanBeInstantiatedPattern : IUnitPattern
  {
    public static readonly IUnitPattern Instance = new CanBeInstantiatedPattern();

    private CanBeInstantiatedPattern() { }

    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return !unitId.Key.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}
