using System;

namespace Armature.Core.Common
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
