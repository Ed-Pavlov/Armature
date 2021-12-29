using System;
using System.Diagnostics;

namespace Armature.Core.Sdk;

public static class ExceptionExtension
{
  [DebuggerStepThrough]
  public static T AddData<T>(this T exception, object key, object? value)
    where T : Exception
  {
    if(exception is null) throw new ArgumentNullException(nameof(exception));
    if(key is null) throw new ArgumentNullException(nameof(key));

    exception.Data.Add(key, value);

    return exception;
  }
}
