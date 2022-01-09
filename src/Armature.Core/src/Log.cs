using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so add a listener to see the log.
/// </summary>
[PublicAPI]
public static class Log
{
  private static DeferredLogScope? _activeDeferredScope;

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
      DoWrite(text, logLevel);
  }

  [StringFormatMethod("format")]
  public static void Write(LogLevel logLevel, string format, params object[] parameters)
  {
    if(_logLevel >= logLevel)
      DoWrite(string.Format(format, parameters), logLevel);
  }

  public static void Write(LogLevel logLevel, [InstantHandle] Func<string> getText)
  {
    if(_logLevel >= logLevel)
      DoWrite(getText(), logLevel);
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1>(LogLevel logLevel, string format, T1 p1)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1), logLevel);
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2>(LogLevel logLevel, string format, T1 p1, T2 p2)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1, p2), logLevel);
  }

  [StringFormatMethod("format")]
  public static void WriteLine<T1, T2, T3>(LogLevel logLevel, string format, T1 p1, T2 p2, T3 p3)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, p1, p2, p3), logLevel);
  }

  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, string format, params object[] parameters)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(string.Format(format, parameters), logLevel);
  }

  public static void WriteLine(LogLevel logLevel, string line)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(line, logLevel);
  }

  /// <summary>
  /// This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
  /// use if calculating arguments for logging takes a time.
  /// </summary>
  [StringFormatMethod("format")]
  public static void WriteLine(LogLevel logLevel, [InstantHandle] Func<string> createMessage)
  {
    if(_logLevel >= logLevel)
      DoWriteLine(createMessage(), logLevel);
  }

  /// <summary>
  /// Used to make an indented "block" in log data
  /// </summary>
  public static Indenter IndentBlock(LogLevel logLevel, string name, string brackets, int indentDelta = 1)
  {
    if(brackets is null) throw new ArgumentNullException(nameof(brackets));
    return _logLevel < logLevel ? Indenter.Empty : new Indenter(name, logLevel, brackets, indentDelta);
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

  public static Disposable ConditionalMode(LogLevel conditionalLevel, Func<bool> predicate)
  {
    if(_logLevel != conditionalLevel) return Disposable.Empty;

    var deferredScope = new DeferredLogScope(predicate);

    if(_activeDeferredScope is not null)
      _activeDeferredScope.AddInnerScope(deferredScope);

    // implement stack by closure
    var prevActiveDeferredScope = _activeDeferredScope;
    _activeDeferredScope = deferredScope;

    return new Disposable(
      () =>
      {
        if(_activeDeferredScope != deferredScope)
        {
          WriteToTrace($"{LogConst.LoggingSubsystemError}: \"\"\"", true);

          WriteToTrace(
            $"Disposing of objects returned from {nameof(Log)}.{nameof(Log.ConditionalMode)} should not be overlapped, object returned "
          + "later should be disposed earlier."
          + Environment.NewLine
          + "Some log data is lost. Fix your code and run it again.",
            true);

          WriteToTrace("\"\"\"", true);
          _activeDeferredScope = null; // drop all data due to it can't be guaranteed to be consistent
          return;
        }

        _activeDeferredScope = prevActiveDeferredScope;

        if(_activeDeferredScope is null) // all deferred scopes are finished, write them to the real log
          deferredScope.WriteToTrace();
      });
  }

  private static void DoWriteLine(string line, LogLevel logLevel) => DoWrite(line, logLevel, true);
  private static void DoWrite(string text, LogLevel logLevel, bool newLine = false)
  {
    if(_activeDeferredScope is not null)
      _activeDeferredScope.Add(text, newLine, logLevel);
    else
      WriteToTrace(text, newLine);
  }

  private static void AmendIndentLevel(int indentDelta, LogLevel logLevel)
  {
    if(_activeDeferredScope is not null)
      _activeDeferredScope.Add("", false, logLevel, indentDelta);
    else
      Trace.IndentLevel += indentDelta;
  }

  private static void WriteToTrace(string text, bool newLine)
  {
    if(newLine)
      Trace.WriteLine(text);
    else
      Trace.Write(text);
  }

  public struct Indenter : IDisposable
  {
    public static Indenter Empty = new();

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    private readonly LogLevel _logLevel;
    private readonly string _brackets;
    private readonly int    _indentDelta;
    private          bool   _isDisposed = false;

    public Indenter(string name, LogLevel logLevel, string brackets, int indentDelta)
    {
      if(brackets.Length is not (0 or 2))
        throw new ArgumentException("String should be empty or contain two simple symbols, at index 0 the opening bracket at index 1 the closing one");

      _logLevel    = logLevel;
      _brackets    = brackets;
      _indentDelta = indentDelta;

      DoWriteLine(name + (_brackets.Length == 0 ? "" : " " + _brackets[0]), logLevel);
      AmendIndentLevel(_indentDelta, _logLevel);
    }

    public void Dispose()
    {
      if(_isDisposed)
      {
        WriteToTrace($"{LogConst.LoggingSubsystemError}: \"\"\"", true);

        WriteToTrace(
          $"Object returned from {nameof(Log)}.{nameof(NamedBlock)} or {nameof(IndentBlock)} disposed more then once, it can indicate "
        + "an error in your code and can lead wrong log output.",
          true);

        WriteToTrace("\"\"\"", true);
      }

      _isDisposed = true;

      if(_brackets is null) return; // Indenter.Empty has _brackets == null

      AmendIndentLevel(-_indentDelta, _logLevel);
      DoWriteLine(_brackets.Length == 0 ? "" : _brackets[1].ToString(), _logLevel);
    }
  }

  public struct Disposable : IDisposable
  {
    public static readonly Disposable Empty = new();

    private readonly Action? _action;

    public Disposable(Action action) => _action = action;

    public void Dispose() => _action?.Invoke();
  }

  private class DeferredLogScope
  {
    private readonly List<Tuple<DeferredLogScope, int>> _innerScopes = new();
    private readonly List<Entry>                        _entries     = new();
    private readonly Func<bool>                         _predicate;

    public DeferredLogScope(Func<bool> predicate) => _predicate = predicate;

    public void Add(string text, bool isLine, LogLevel logLevel, int indent = 0) => _entries.Add(new Entry(text, isLine, logLevel, indent));

    public void AddInnerScope(DeferredLogScope innerScope) => _innerScopes.Add(Tuple.Create(innerScope, _entries.Count));

    public void WriteToTrace()
    {
      var shouldWrite = _predicate();
      var startIndex  = 0;

      foreach(var (innerScope, endIndex) in _innerScopes)
      {
        WriteEntries(startIndex, endIndex, shouldWrite);
        innerScope.WriteToTrace();
        startIndex = endIndex;
      }

      WriteEntries(startIndex, _entries.Count, shouldWrite);
    }

    private void WriteEntries(int start, int end, bool shouldWrite)
    {
      for(var i = start; i < end; i++)
      {
        var entry = _entries[i];

        if(_logLevel > entry.LogLevel || (_logLevel == entry.LogLevel && shouldWrite))
        {
          if(entry.IndentDelta != 0)
            Trace.IndentLevel += entry.IndentDelta; // entry with IndentDelta never contains data to write and vice versa
          else
            Log.WriteToTrace(entry.Text, entry.IsLine);
        }
      }
    }

    private struct Entry
    {
      public Entry(string text, bool isLine, LogLevel logLevel, int indentDelta)
      {
        Text        = text;
        IndentDelta = indentDelta;
        IsLine      = isLine;
        LogLevel    = logLevel;
      }

      public readonly LogLevel LogLevel;
      public readonly int      IndentDelta;
      public readonly string   Text;
      public readonly bool     IsLine;
    }
  }
}

public enum LogLevel { None = 0, Info, Verbose, Trace }