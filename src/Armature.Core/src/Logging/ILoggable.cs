using Armature.Core.Annotations;

namespace Armature.Core;

/// <summary>
/// Represents an object as a text ready to be logged.
/// </summary>
public interface ILogString
{
  [WithoutTest]
  string ToHoconString();
}

/// <summary>
/// An object is able to print its content to the log not as a part of runtime logging but as an entity with content.
/// </summary>
public interface ILoggable
{
  [WithoutTest]
  void PrintToLog(LogLevel logLevel = LogLevel.None);
}