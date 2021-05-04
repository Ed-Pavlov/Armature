namespace Armature.Core
{
  public static class Static<T> where T : new()
  {
    public static readonly T Instance = new ();
  }
}
