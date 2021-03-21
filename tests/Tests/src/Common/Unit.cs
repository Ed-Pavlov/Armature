using Armature.Core;

namespace Tests.Common
{
  internal static class Unit
  {
    public static UnitInfo OfType<T>(object token = null) => new(typeof(T), token);
  }
}