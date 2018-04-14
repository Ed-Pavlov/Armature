namespace Armature.Core
{
  /// <summary>
  /// Not equality member are needed for this class, <see cref="_name"/> is used only for debug purpose, tokens should be equal by reference.
  /// </summary>
  public class Token
  {
    public static readonly Token Any = new Token("Any");
    public static readonly Token Propagate = new Token("Propagate");
    
    private readonly string _name;
    public Token(string name) => _name = name;
    
    public override string ToString() => string.Format(typeof(Token).Name + "." + _name);
  }
}