using System;

namespace Armature.Core
{
  public static class TokenExtension
  {
    public static bool Matches(this object? matcherToken, object? unitToken)
    {
      if(ReferenceEquals(unitToken, Token.Any))
        throw new ArgumentOutOfRangeException(
          nameof(unitToken),
          "Building unit token can't be 'Any' special token. Check arguments order in the call of this method");

      return Equals(matcherToken, unitToken) || Equals(matcherToken, Token.Any);
    }
  }
}
