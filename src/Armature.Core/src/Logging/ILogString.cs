namespace Armature.Core.Logging
{
  public interface ILogString
  {
    string ToHoconString();
  }

  public interface ILogPrintable
  {
    void PrintToLog(LogLevel logLevel = LogLevel.None);
  }
}