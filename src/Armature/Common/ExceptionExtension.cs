using System;
using JetBrains.Annotations;

namespace Armature.Common
{
  public static class ExceptionExtension
  {
    public static T AddData<T>([NotNull] this T exception, [NotNull] object key, [CanBeNull] object value) where T : Exception
    {
      if (exception == null) throw new ArgumentNullException("exception");
      if (key == null) throw new ArgumentNullException("key");
      
      exception.Data.Add(key, value);
      return exception;
    }
  }
}