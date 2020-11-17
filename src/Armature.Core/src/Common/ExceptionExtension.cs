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

    public static ArmatureException Aggregate(this IReadOnlyCollection<Exception> exceptions, string baseMessage)
    {
      var message = new StringBuilder(baseMessage);
      message.AppendLine();

      var i = 0;
      foreach (var exc in exceptions)
      {
        message.AppendLine($"Exception#{++i}: {exc.Message}");
        message.AppendLine(exc.StackTrace);
        message.AppendLine("----------------------------------------------------");
        message.AppendLine();
      }

      var exception = new ArmatureException(message.ToString());
      i = 0;
      foreach (var exc in exceptions)
      {
        var exceptionInfo = exc.Message + Environment.NewLine + exc.StackTrace;
        exception.AddData(++i, exceptionInfo);
      }
      return exception;
    }
  }
}