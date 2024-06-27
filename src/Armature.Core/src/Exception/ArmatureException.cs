using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BeatyBit.Armature.Core;

/// <summary>
/// Exception is used to distinguish internal unexpected situations and error produced by user code.
/// </summary>
[Serializable]
public class ArmatureException : AggregateException
{
  [DebuggerStepThrough]
  public ArmatureException() : base(LogConst.ArmatureExceptionPostfix()){}

  [DebuggerStepThrough]
  public ArmatureException(string message) : base(message + LogConst.ArmatureExceptionPostfix()) { }

  [DebuggerStepThrough]
  public ArmatureException(string message, Exception innerException) : base(message + LogConst.ArmatureExceptionPostfix(), innerException) { }

  [DebuggerStepThrough]
  public ArmatureException(string message, IEnumerable<Exception> innerExceptions) : base(message, innerExceptions) { }

  [DebuggerStepThrough]
  public override string ToString()
  {
    var baseValue = base.ToString();

    if(Data.Count == 0)
      return baseValue;

    var sb = new StringBuilder(baseValue).AppendLine("Exception data:");

    var i = 0;
    foreach(DictionaryEntry pair in Data)
      sb.AppendFormat("\tRecord {0}:", i++)
        .AppendFormat("Key: {0}, Value={1}", pair.Key, pair.Value)
        .AppendLine();

    return sb.ToString();
  }
}