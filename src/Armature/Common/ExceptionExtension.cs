using System;
using System.Diagnostics;
using Resharper.Annotations;

namespace Armature.Common
{
  internal static class ExceptionExtension
  {
    [DebuggerStepThrough]
    public static T AddData<T>([NotNull] this T exception, [NotNull] object key, [CanBeNull] object value)
      where T : Exception
    {
      if (exception == null) throw new ArgumentNullException(nameof(exception));
      if (key == null) throw new ArgumentNullException(nameof(key));

      exception.Data.Add(key, value);
      return exception;
    }
  }
}