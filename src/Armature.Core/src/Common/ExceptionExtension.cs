using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace Armature.Core.Common
{
  internal static class ExceptionExtension
  {
    [DebuggerStepThrough]
    public static T AddData<T>(this T exception, object key, object? value)
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

      string ExceptionTitle(Exception exc) => $"{exc.GetType()}: {exc.Message}";

      var i = 0;
      foreach (var exc in exceptions)
      {
        message.AppendLine($"Exception#{++i} {ExceptionTitle(exc)}");
        message.AppendLine(exc.StackTrace);
        message.AppendLine("----------------------------------------------------");
        message.AppendLine();
      }

      var exception = new ArmatureException(message.ToString());
      i = 0;
      foreach (var exc in exceptions)
      {
        var exceptionInfo = ExceptionTitle(exc) + Environment.NewLine + exc.StackTrace;
        exception.AddData(++i, exceptionInfo);
      }
      return exception;
    }
  }
}