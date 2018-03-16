using System;
using System.Linq;
using JetBrains.Annotations;

namespace Armature.Logging
{
  public static class Log
  {
    private static int _indent;
    private static LogLevel _logLevel;

    public static IDisposable Enabled(LogLevel logLevel = LogLevel.Info)
    {
      _logLevel = logLevel;
      return new Bracket(() => _logLevel = logLevel, () => _logLevel = LogLevel.None);
    }

    public static IDisposable Block(LogLevel logLevel, string name, params object[] parameters)
    {
      WriteLine(logLevel, name, parameters);
      return logLevel <= _logLevel ? AddIndent(true) : new DumbDisposable();
    }

    public static bool LogNamespace { get; } = false;
    
    public static IDisposable AddIndent(bool newBlock = false, int count = 1) => new Indenter(newBlock, count);
    
    [StringFormatMethod("format")]
    public static void Info(string format, params object[] parameters) => WriteLine(LogLevel.Info, format, parameters);

    [StringFormatMethod("format")]
    public static void Trace(string format, params object[] parameters) => WriteLine(LogLevel.Trace, format, parameters);

    [StringFormatMethod("format")]
    public static void Verbose(string format, params object[] parameters) => WriteLine(LogLevel.Verbose, format, parameters);

    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, string format, params object[] parameters)
    {
      if (logLevel > _logLevel) return;

      // ReSharper disable once CoVariantArrayConversion
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, parameters.Select(ToLogString).ToArray()));
    }

    private static string GetIndent()
    {
      var indent = "";
      for (var i = 0; i < _indent; i++)
        indent += "  ";
      return indent;
    }

    public static string AsLogString(this Type type) => LogNamespace ? type.ToString() : type.GetShortName();
    private static string AsLogString(object obj)
    {
      if (obj == null) return "null";

      // if ToString is not overriden and returns object type full name - return type.AsLogString
      var toString = obj.ToString();
      var type = obj.GetType();
      return toString == GetTypeFullName(type) ? type.AsLogString() : toString;
    }

    private static string GetTypeFullName(Type type)
    {
      if (!type.IsGenericType) return type.FullName;

      var main = type.GetGenericTypeDefinition().FullName;
      var arguments = string.Join(", ", type.GenericTypeArguments.Select(GetTypeFullName).ToArray());
      return string.Format("{0}[{1}]", main, arguments);
    }
    
    internal static string ToLogString(object obj) => obj == null ? "null" : AsLogString((dynamic)obj);

    private class Indenter : IDisposable
    {
      private readonly int _count;
      private readonly bool _newBlock;

      public Indenter(bool newBlock, int count)
      {
        if (newBlock)
          Info("{{");

        _newBlock = newBlock;
        _count = count;
        _indent += count;
      }

      public void Dispose()
      {
        _indent -= _count;
        if (_newBlock)
          Info("}}");
      }
    }

    private class Bracket : IDisposable
    {
      private readonly Action _endAction;

      public Bracket(Action startAction, Action endAction)
      {
        startAction();
        _endAction = endAction;
      }

      public void Dispose() => _endAction();
    }

    private class DumbDisposable : IDisposable
    {
      public void Dispose()
      {
        // dumb
      }
    }
  }

  public enum LogLevel
  {
    None = 0,
    Info,
    Verbose,
    Trace
  }
}