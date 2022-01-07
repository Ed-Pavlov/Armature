using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so
/// add a listener to see the log.
/// </summary>
[PublicAPI]
public static class Log
{
  private static readonly Stack<List<string>> DeferredContent = new();

  private static LogLevel _logLevel = LogLevel.None;

  /// <summary>
  /// Set should full type name be logged or only short name w/o namespace to simplify reading.
  /// </summary>
  public static bool LogFullTypeName = false;

  static Log() => IndentSize = 2;

  /// <summary>
  /// The count of spaces used to indent lines
  /// </summary>
  public static int IndentSize
  {
    get => Trace.IndentSize;
    set => Trace.IndentSize = value;
  }

  /// <summary>
  /// Used to enable logging, can be limited by disposing returned object
  /// </summary>
  public static Disposable Enable(LogLevel logLevel = LogLevel.Info)
  {
    var prevLevel = _logLevel;
    _logLevel = logLevel;
    return new Disposable(() => _logLevel = prevLevel);
  }

  /// <summary>
  /// Used to disable logging, can be limited by disposing returned object
  /// </summary>
  public static Disposable Disable()
  {
    var prevLevel = _logLevel;
    _logLevel = LogLevel.None;
    return new Disposable(() => _logLevel = prevLevel);
  }

  public static void Write(LogLevel logLevel, string text)
  {
    if(_logLevel >= logLevel)
      DoWrite(text);
  }

  [StringFormatMethod("format")]
  public static void Write(LogLevel logLevel, string format, params object[] parameters)
  {
    if(_logLevel >= logLevel)
      DoWrite(string.Format(format, parameters));
  }

  public static void Write(LogLevel logLevel, [InstantHandle] Func<string> getText)
  {
    if(_logLevel >= logLevel)
      DoWrite(getText());
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1>(LogLevel logLevel, string format, T1 p1)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1));
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2>(LogLevel logLevel, string format, T1 p1, T2 p2)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1, p2));
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2, T3>(LogLevel logLevel, string format, T1 p1, T2 p2, T3 p3)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1, p2, p3));
  }

  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, string format, params object[] parameters)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, parameters));
  }

  public static void WriteLine(LogLevel logLevel, string line)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(line);
  }

  /// <summary>
  /// This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
  /// use if calculating arguments for logging takes a time.
  /// </summary>
  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, [InstantHandle] Func<string> createMessage)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(createMessage());
  }

  /// <summary>
  /// Used to make an indented "block" in log data
  /// </summary>
  public static Indenter IndentBlock(LogLevel logLevel, string name, string brackets, int count = 1)
  {
    if(_logLevel < logLevel) return Indenter.Empty;

    DoWrite(name);
    return new Indenter(brackets, count);
  }

  /// <summary>
  /// Used to make a named and indented "block" in log data
  /// </summary>
  public static Indenter NamedBlock(LogLevel logLevel, string name) => _logLevel >= logLevel ? IndentBlock(logLevel, name, "{}") : Indenter.Empty;

  /// <summary>
  /// Used to make a named and indented "block" in log data
  /// </summary>
  [StringFormatMethod("format")]
  public static Indenter NamedBlock(LogLevel logLevel, string format, params object[] parameters)
    => _logLevel >= logLevel ? IndentBlock(logLevel, string.Format(format, parameters), "{}") : Indenter.Empty;

  /// <summary>
  /// Used to make a named and indented "block" in log data
  /// </summary>
  public static Indenter NamedBlock(LogLevel logLevel, [InstantHandle] Func<string> getName)
    => _logLevel >= logLevel ? IndentBlock(logLevel, getName(), "{}") : Indenter.Empty;

  /// <summary>
  /// Executes action if <paramref name="logLevel"/> satisfies current Log level. See <see cref="Enable"/> for details
  /// </summary>
  /// <param name="logLevel"></param>
  /// <param name="action"></param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Execute(LogLevel logLevel, [InstantHandle] Action action)
  {
    if(_logLevel >= logLevel)
      action();
  }

  public static Disposable Deferred(LogLevel logLevel, Action<Action?> action)
  {
    if(_logLevel < logLevel) return Disposable.Empty;

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

  public struct Indenter : IDisposable
  {
    public static Indenter Empty = new();

    private readonly string _brackets;
    private readonly int    _indent;
    public Indenter(string brackets, int indent)
    {
      _brackets = brackets;
      _indent   = indent;
      Open(brackets, indent);
    }

    public void Dispose()
    {
      if(_brackets is not null)
        Close(_brackets, _indent);
    }

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

  public struct Disposable : IDisposable
  {
    public static readonly Disposable Empty = new();

    private readonly Action? _action;

    public Disposable(Action action) => _action = action;

    public void Dispose() => _action?.Invoke();
  }
}

public enum LogLevel { None = 0, Info, Verbose, Trace }