using System.Reflection;
using BeatyBit.Armature.Core;
using JetBrains.Annotations;

namespace BeatyBit.Armature.Sdk;

[PublicAPI]
public static class LogExtension
{
  public static void WriteToLog(this ConstructorInfo? constructor, LogLevel logLevel)
  {
    if(Log.IsEnabled(logLevel))
      Log.WriteLine(logLevel, $"Constructor: {constructor.ToHoconString()}");
  }
}
