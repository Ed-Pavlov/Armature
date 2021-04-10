using Armature.Core;

namespace Tests.Common
{
  internal static class Unit
  {
    public static UnitId OfType<T>(object token = null) => new(typeof(T), token);
  }
}
