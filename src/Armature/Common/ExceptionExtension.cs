using System;

namespace Armature.Common
{
  public static class ExceptionExtension
  {
    public static T AddData<T>(this T exception, object key, object value) where T : Exception
    {
      exception.Data.Add(key, value);
      return exception;
    }
  }
}