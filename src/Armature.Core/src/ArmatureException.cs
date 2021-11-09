using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Armature.Core;

/// <summary>
///   Exception is used to distinguish internal unexpected situations and error produced by user code
/// </summary>
[Serializable]
public class ArmatureException : ApplicationException
{
  private static readonly string Postfix =
    Environment.NewLine
  + $"See {nameof(Exception)}.{nameof(Data)} for details or enable logging using {nameof(Log)}.{nameof(Log.Enable)} to investigate the error.";

  [DebuggerStepThrough]
  public ArmatureException(string message) : base(message + Postfix) { }

  [DebuggerStepThrough]
  public ArmatureException(string message, Exception innerException) : base(message + Postfix, innerException) { }

  [DebuggerStepThrough]
  public override string ToString()
  {
    var baseValue = base.ToString();

    if(Data.Count == 0)
      return baseValue;

    var sb = new StringBuilder(baseValue)
     .AppendLine("Exception data:");

    var i = 0;

    foreach(DictionaryEntry pair in Data)
      sb.AppendFormat("\tRecord {0}:", i++)
        .AppendFormat("Key: {0}, Value={1}", pair.Key, pair.Value)
        .AppendLine();

    return sb.ToString();
  }
}