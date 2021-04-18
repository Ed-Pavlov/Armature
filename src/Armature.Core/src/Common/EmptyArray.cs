namespace Armature.Core
{
  internal static class EmptyArray<T>
  {
    // ReSharper disable once InconsistentNaming
    private static readonly T[]? _instance;
    public static readonly  T[]  Instance = _instance ??= new T[0];
  }
}
