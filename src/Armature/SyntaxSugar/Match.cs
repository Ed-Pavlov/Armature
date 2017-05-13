using System;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature
{
  public static class Match
  {
    public static UnitInfoMatcher Type<T>(object token)
    {
      return Type(typeof(T), token);
    }

    public static UnitInfoMatcher Type([NotNull] Type type, object token)
    {
      if (type == null) throw new ArgumentNullException("type");
      return new UnitInfoMatcher(
        new UnitInfo(type, token),
        (pattern, other) => pattern.GetUnitTypeSafe() == other.GetUnitTypeSafe() && Equals(pattern.Token, other.Token),
        PassingBuildSequenceWeight.WeakSequence);
    }

    public static UnitInfoMatcher OpenGenericType(Type type, object token)
    {
      return new UnitInfoMatcher(
        new UnitInfo(type, token),
        (pattern, other) =>
          {
            var unitType = other.GetUnitTypeSafe();
            return unitType != null && unitType.IsGenericType && Equals(unitType.GetGenericTypeDefinition(), pattern.Id);
          },
        PassingBuildSequenceWeight.WeakSequenceOpenGeneric);
    }
  }
}