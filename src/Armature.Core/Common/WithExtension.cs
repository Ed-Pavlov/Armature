using System;

namespace Armature.Common
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