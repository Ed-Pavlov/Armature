using System;

namespace Tests.Common
{
  public static class WithExtension
  {
    public static T With<T>(this T obj, Action<T> action)
    {
      action(obj);

      return obj;
    }
  }
}
