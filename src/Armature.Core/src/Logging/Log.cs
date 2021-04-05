using System;
using System.Linq;
using JetBrains.Annotations;

namespace Armature.Core.Logging
{
  /// <summary>
  ///   Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so
  ///   add a listener to see the log.
  /// </summary>
  public static class Log
  {
    private static int      _indent;
    private static LogLevel _logLevel = LogLevel.None;

    /// <summary>
    ///   Set should full type name be logged or only short name w/o namespace to simplify reading.
    /// </summary>
    public static bool LogFullTypeName { get; } = false;

    /// <summary>
    ///   Used to enable logging in a limited scope using "using" C# keyword
    /// </summary>
    public static IDisposable Enabled(LogLevel logLevel = LogLevel.Info)
    {
      _logLevel = logLevel;

      return new Bracket(() => _logLevel = logLevel, () => _logLevel = LogLevel.None);
    }

    /// <summary>
    ///   Used to make an indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel) => logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel, string name, params object[] parameters)
    {
      WriteLine(logLevel, name, parameters);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel, Func<string> getName)
    {
      WriteLine(logLevel, getName);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block<T1>(LogLevel logLevel, Func<T1, string> getName, T1 v1)
    {
      WriteLine(logLevel, getName, v1);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make an indented "block" in log data
    /// </summary>
    public static IDisposable AddIndent(bool newBlock = false, int count = 1) => new Indenter(newBlock, count);

    [StringFormatMethod("format")]
    public static void Info(string format, params object[] parameters) => WriteLine(LogLevel.Info, format, parameters);

    [StringFormatMethod("format")]
    public static void Trace(string format, params object[] parameters) => WriteLine(LogLevel.Trace, format, parameters);

    [StringFormatMethod("format")]
    public static void Verbose(string format, params object[] parameters) => WriteLine(LogLevel.Verbose, format, parameters);

    /// <summary>
    /// Executes action if <paramref name="logLevel"/> satisfies current Log level. See <see cref="Enabled"/> for details
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="action"></param>
    public static void ExecuteIfEnabled(this LogLevel logLevel, Action action)
    {
      if(logLevel > _logLevel) return;

      action();
    }

    public static void WriteLine<T1>(LogLevel logLevel, string format, T1 p1)
    {
      if(logLevel > _logLevel) return;
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, p1.ToLogString()));
    }

    public static void WriteLine<T1, T2>(LogLevel logLevel, string format, T1 p1, T2 p2)
    {
      if(logLevel > _logLevel) return;
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, p1.ToLogString(), p2.ToLogString()));
    }

    public static void WriteLine<T1, T2, T3>(LogLevel logLevel, string format, T1 p1, T2 p2, T3 p3)
    {
      if(logLevel > _logLevel) return;
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, p1.ToLogString(), p2.ToLogString(), p3.ToLogString()));
    }

    [StringFormatMethod("format")]
    public static void Write(LogLevel logLevel, string format, params object[] parameters)
    {
      if(logLevel > _logLevel) return;

      // ReSharper disable once CoVariantArrayConversion
      System.Diagnostics.Trace.Write(string.Format(format, parameters.Select(ToLogString).ToArray()));
    }

    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, string format, params object[] parameters)
    {
      if(logLevel > _logLevel) return;

      // ReSharper disable once CoVariantArrayConversion
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, parameters.Select(ToLogString).ToArray()));
    }

    /// <summary>
    ///   This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
    ///   use it calculating arguments for logging takes a time.
    /// </summary>
    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, [InstantHandle] Func<string> createMessage)
    {
      if(logLevel > _logLevel) return;

      System.Diagnostics.Trace.WriteLine(GetIndent() + createMessage());
    }

    /// <summary>
    ///   This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
    ///   use it calculating arguments for logging takes a time.
    /// </summary>
    [StringFormatMethod("format")]
    public static void WriteLine<T1>(LogLevel logLevel, [InstantHandle] Func<T1, string> createMessage, T1 v1)
    {
      if(logLevel > _logLevel) return;

      System.Diagnostics.Trace.WriteLine(GetIndent() + createMessage(v1));
    }

    /// <summary>
    ///   Returns the name of <paramref name="type" /> respecting <see cref="LogFullTypeName" /> property
    /// </summary>
    public static string ToLogString(this Type type) => LogFullTypeName ? GetTypeFullName(type) : type.GetShortName();

    /// <summary>
    ///   Returns log representation of object, some objects logs in more friendly form then common <see cref="object.ToString" /> returns
    /// </summary>
    public static string ToLogString(this object? obj)
    {
      if(obj is null) return "null";

      return obj is Type type ? type.ToLogString() : obj.ToString();
    }

    private static string GetIndent()
    {
      var indent = "";

      for(var i = 0; i < _indent; i++)
        indent += "  ";

      return indent;
    }

    private static string GetTypeFullName(Type type)
    {
      if(!type.IsGenericType) return type.FullName!;

      var main      = type.GetGenericTypeDefinition().FullName;
      var arguments = string.Join(", ", type.GenericTypeArguments.Select(GetTypeFullName).ToArray());

      return string.Format("{0}[{1}]", main, arguments);
    }

    private class Indenter : IDisposable
    {
      private readonly int  _count;
      private readonly bool _newBlock;

      public Indenter(bool newBlock, int count)
      {
        if(newBlock)
          WriteLine(LogLevel.Info, "{{");

        _newBlock =  newBlock;
        _count    =  count;
        _indent   += count;
      }

      public void Dispose()
      {
        _indent -= _count;

        if(_newBlock)
          WriteLine(LogLevel.Info, "}}");
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
      public static readonly IDisposable Instance = new DumbDisposable();

      private DumbDisposable() { }

      public void Dispose()
      {
        // dumb
      }
    }
  }

  public enum LogLevel { None = 0, Info, Verbose, Trace }
}
