namespace Armature.Core
{
  /// <summary>
  ///   Not equality member are needed for this class, <see cref="_name" /> is used only for debug purpose, tokens should be equal by reference.
  /// </summary>
  public class Token
  {
    /// <summary>
    ///   Means "any token"
    /// </summary>
    public static readonly Token Any = new("Any");

    /// <summary>
    ///   Used to propagate token to building dependencies
    /// </summary>
    public static readonly Token Propagate = new("Propagate");

    private readonly string _name;
    public Token(string name) => _name = name;

    public override string ToString() => string.Format(nameof(Token) + "." + _name);
  }
}
