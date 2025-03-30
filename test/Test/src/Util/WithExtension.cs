using System;

namespace Armature.Test.Util
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