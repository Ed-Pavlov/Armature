namespace Armature.Extensibility;

public static class ExtensibilityExtension
{
  /// <summary>
  ///   Usually inheritors of <see cref="IUnitSequenceExtensibility" /> explicitly to not expose service method to the public interface.
  ///   This method simplifies casting of inheritors of <see cref="IUnitSequenceExtensibility" /> to this interface.
  /// </summary>
  /// <param name="ext"></param>
  /// <returns></returns>
  public static T AsExtensibility<T>(this T ext) => ext;
}