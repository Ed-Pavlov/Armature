namespace Tests.Common
{
  public static class Extension
  {
    public static T[] AsArray<T>(this T self) => new[] {self};
  }
}
