namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Reusable logic of matching a token of the <see cref="UnitInfo"/> in <see cref="IUnitMatcher"/> 
  /// </summary>
  public abstract class UnitInfoMatcherBase
  {
    protected readonly object? Token;

    protected UnitInfoMatcherBase(object? token) => Token = token;

    protected bool MatchesToken(UnitInfo unitInfo) => Equals(Token, unitInfo.Token) || Equals(Token, Core.Token.Any);
  }
}