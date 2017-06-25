using System;
using System.Collections;
using System.Text;

namespace Armature.Core
{
  [Serializable]
  public class ArmatureException : ApplicationException
  {
    public ArmatureException(string message) : base(message)
    {}

    public ArmatureException(string message, Exception innerException) : base(message, innerException)
    {}

    public override string ToString()
    {
      var baseValue = base.ToString();
      if (Data.Count == 0)
        return baseValue;

      var sb = new StringBuilder(baseValue)
        .AppendLine("Exception data:");

      var i = 0;
      foreach (DictionaryEntry pair in Data)
      {
        sb.AppendFormat("\tRecord {0}:", i++)
          .AppendFormat("Key: {0}, Value={1}", pair.Key, pair.Value)
          .AppendLine();
      }
      return sb.ToString();
    }
  }
}