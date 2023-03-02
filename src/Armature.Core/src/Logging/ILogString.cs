using Armature.Core.Annotations;

namespace Armature.Core;

public interface ILogString
{
  [WithoutTest]
  string ToHoconString();
}

public interface ILogPrintable
{
  [WithoutTest]
  void PrintToLog(LogLevel logLevel = LogLevel.None);
}