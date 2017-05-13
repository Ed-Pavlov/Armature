namespace Armature.Core
{
  /// <summary>
  /// Represents a result of the <see cref="Build"/>, null is a valid result.
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