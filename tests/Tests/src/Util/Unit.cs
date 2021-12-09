using System;
using Armature.Core;

namespace Tests.Util
{
  internal class Unit
  {
    public static UnitId IsType<T>() => new UnitId(typeof(T), null);

    public static UnitId IsType(Type type) => new UnitId(type, null);
    public static UnitId Is(object?  kind) => new UnitId(kind, null);
  }
}