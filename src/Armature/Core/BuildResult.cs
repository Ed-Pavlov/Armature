namespace Armature.Core
{
  /// <summary>
  /// Represents a result of the <see cref="BuildSession"/>, null is a valid <see cref="Value"/>.
  /// </summary>
  public class BuildResult
  {
    public readonly object Value;

    public BuildResult(object value)
    {
      Value = value;
    }

    public override string ToString()
    {
      return Value == null ? "null" : Value.ToString();
    }
  }
}