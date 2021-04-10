using Armature.Core;

namespace Tests.Common
{
  internal static class Unit
  {
    public static UnitId OfType<T>(object key = null) => new(typeof(T), key);
  }
}
