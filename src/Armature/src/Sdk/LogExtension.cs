using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Sdk;

[PublicAPI]
public static class LogExtension
{
  public static void WriteToLog(this ConstructorInfo? constructor, LogLevel logLevel)
  {
    if(Log.IsEnabled(logLevel))
      Log.WriteLine(logLevel, $"Constructor: {constructor.ToHoconString()}");
  }
}
