namespace Armature.Core.Sdk
{
  public static class Static
  {
    public static T Of<T>() where T : new() => Factory<T>.Instance;

    private static class Factory<T> where T : new()
    {
      public static readonly T Instance = new();
    }
  }
}