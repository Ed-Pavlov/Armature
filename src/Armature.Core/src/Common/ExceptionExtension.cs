using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace Armature.Core.Common
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

    public static ArmatureException Aggregate(this IEnumerable<Exception> exceptions, string baseMessage)
    {
      var message = new StringBuilder(baseMessage);
      message.AppendLine();

      var i = 0;
      foreach (var exc in exceptions) 
        message.AppendLine($"Exception#{++i}: {exc.Message}");

      var exception = new ArmatureException(message.ToString());
      i = 0;
      foreach (var exc in exceptions)
        exception.AddData(++i, exc);
      
      return exception;
    }
  }
}