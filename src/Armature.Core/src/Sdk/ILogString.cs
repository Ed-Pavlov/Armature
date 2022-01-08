using Armature.Core.Annotations;

namespace Armature.Core.Sdk;

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