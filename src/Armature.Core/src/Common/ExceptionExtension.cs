using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Armature.Core.Logging;

namespace Armature.Core
{
  internal static class ExceptionExtension
  {
    [DebuggerStepThrough]
    public static T AddData<T>(this T exception, object key, object? value)
      where T : Exception
    {
      if(exception is null) throw new ArgumentNullException(nameof(exception));
      if(key is null) throw new ArgumentNullException(nameof(key));

      exception.Data.Add(key, value.ToLogString());

      return exception;
    }

    public static ArmatureException Aggregate(this IReadOnlyCollection<Exception> exceptions, string baseMessage)
    {
      var message = new StringBuilder(baseMessage);
      message.AppendLine();

      var i = 0;

      foreach(var exc in exceptions)
      {
        message.AppendLine($"Exception#{++i}");
        message.AppendLine(exc.ToString());
        message.AppendLine("----------------------------------------------------");
        message.AppendLine();
      }

      var exception = new ArmatureException(message.ToString());
      i = 0;

      foreach(var exc in exceptions)
        exception.AddData($"Exception#{++i}", exc);

      return exception;
    }
  }
}
