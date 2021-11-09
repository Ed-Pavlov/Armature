using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
///   Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so
///   add a listener to see the log.
/// </summary>
[PublicAPI]
public static class Log
{
  private static readonly Stack<List<string>> DeferredContent = new();

  private static LogLevel _logLevel = LogLevel.None;

  /// <summary>
  ///   Set should full type name be logged or only short name w/o namespace to simplify reading.
  /// </summary>
  public static bool LogFullTypeName = false;

  static Log() => IndentSize = 2;

  /// <summary>
  ///   The count of spaces used to indent lines
  /// </summary>
  public static int IndentSize
  {
    get => Trace.IndentSize;
    set => Trace.IndentSize = value;
  }

  /// <summary>
  ///   Used to enable logging in a limited scope using "using" C# keyword
  /// </summary>
  public static IDisposable Enable(LogLevel logLevel = LogLevel.Info)
  {
    var prevLevel = _logLevel;
    _logLevel = logLevel;
    return new Disposable(() => _logLevel = prevLevel);
  }

  public static IDisposable Disable()
  {
    var prevLevel = _logLevel;
    _logLevel = LogLevel.None;
    return new Disposable(() => _logLevel = prevLevel);
  }

  public static void Write(LogLevel logLevel, string text)
  {
    if(logLevel > _logLevel) return;

    DoWrite(text);
  }

  [StringFormatMethod("format")]
  public static void Write(LogLevel logLevel, string format, params object[] parameters)
  {
    if(logLevel > _logLevel) return;

    DoWrite(string.Format(format, parameters));
  }

  public static void Write(LogLevel logLevel, Func<string> getText)
  {
    if(logLevel > _logLevel) return;

    DoWrite(getText());
  }

  public static void WriteLine(LogLevel logLevel, string line)
  {
    if(logLevel > _logLevel) return;

    DoWriteLine(line);
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1>(LogLevel logLevel, string format, T1 p1) => WriteLine(logLevel, string.Format(format, p1));

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2>(LogLevel logLevel, string format, T1 p1, T2 p2) => WriteLine(logLevel, string.Format(format, p1, p2));

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2, T3>(LogLevel logLevel, string format, T1 p1, T2 p2, T3 p3)
    => WriteLine(logLevel, string.Format(format, p1, p2, p3));

  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, string format, params object[] parameters) => WriteLine(logLevel, string.Format(format, parameters));

  /// <summary>
  ///   This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
  ///   use it calculating arguments for logging takes a time.
  /// </summary>
  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, [InstantHandle] Func<string> createMessage) => WriteLine(logLevel, createMessage());

  /// <summary>
  ///   Used to make an indented "block" in log data
  /// </summary>
  public static IDisposable IndentBlock(LogLevel logLevel, string name, string brackets, int count = 1)
  {
    if(_logLevel < logLevel) return DumbDisposable.Instance;

    DoWrite(name);
    return new Indenter(brackets, count);
  }

  /// <summary>
  ///   Used to make a named and indented "block" in log data
  /// </summary>
  [StringFormatMethod("format")]
  public static IDisposable NamedBlock(LogLevel logLevel, string name) => IndentBlock(logLevel, name, "{}");

  /// <summary>
  ///   Used to make a named and indented "block" in log data
  /// </summary>
  [StringFormatMethod("format")]
  public static IDisposable NamedBlock(LogLevel logLevel, string format, params object[] parameters)
  {
    if(_logLevel < logLevel) return DumbDisposable.Instance;
    return IndentBlock(logLevel, string.Format(format, parameters), "{}");
  }

  /// <summary>
  ///   Used to make a named and indented "block" in log data
  /// </summary>
  public static IDisposable NamedBlock(LogLevel logLevel, Func<string> getName)
  {
    if(_logLevel < logLevel) return DumbDisposable.Instance;
    return IndentBlock(logLevel, getName(), "{}");
  }

  /// <summary>
  /// Executes action if <paramref name="logLevel"/> satisfies current Log level. See <see cref="Enable"/> for details
  /// </summary>
  /// <param name="logLevel"></param>
  /// <param name="action"></param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Execute(LogLevel logLevel, Action action)
  {
    if(logLevel > _logLevel) return;

    action();
  }


  public static IDisposable Deferred(LogLevel logLevel, Action<Action?> action)
  {
    if(_logLevel < logLevel) return DumbDisposable.Instance;

    DeferredContent.Push(new List<string>());
    var originalIndent = Trace.IndentLevel;

    return new Disposable(
      () =>
      {
        var deferredContent = DeferredContent.Pop();

        action(
          deferredContent.Count == 0
            ? null
            : () =>
              {
                var gap = new string(' ', Trace.IndentLevel - originalIndent);

                foreach(var line in deferredContent)
                  DoWriteLine(gap + line);
              });
      });
  }

  private static void DoWriteLine(string line) => DoWrite(line, true);
  private static void DoWrite(string text, bool newLine = false)
  {
    // all "Write" methods should at the end call this method, so we check for deferred in one place only
    if(DeferredContent.Count > 0)
      DeferredContent.Peek().Add(text);
    else
    {
      if(newLine)
        Trace.WriteLine(text);
      else
        Trace.Write(text);
    }
  }

  private class Indenter : Disposable
  {
    public Indenter(string brackets, int indent) : base(() => Close(brackets, indent)) => Open(brackets, indent);

    private static void Open(string brackets, int indent)
    {
      if(brackets.Length is not (0 or 2))
        throw new ArgumentException("String should be empty or contain two simple symbols, at index 0 the opening bracket at index 1 the closing one");

      if(brackets.Length > 0)
        WriteLine(LogLevel.Info, " " + brackets[0] + " "); //TODO: is there is need to improve the performance of string concatenation?

      Trace.IndentLevel += indent;
    }

    private static void Close(string brackets, int indent)
    {
      Trace.IndentLevel -= indent;

      if(brackets.Length > 0)
        WriteLine(LogLevel.Info, brackets[1].ToString());
    }
  }

  private class Disposable : IDisposable
  {
    private readonly Action _action;

    public Disposable(Action action) => _action = action;

    public void Dispose() => _action();
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