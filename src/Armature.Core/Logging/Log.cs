using System;
using System.Linq;
using Resharper.Annotations;

namespace Armature.Core.Logging
{
  /// <summary>
  ///   Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so
  ///   add a listener to see the log.
  /// </summary>
  public static class Log
  {
    private static int _indent;
    private static LogLevel _logLevel;

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
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel, string name, params object[] parameters)
    {
      WriteLine(logLevel, name, parameters);
      return logLevel <= _logLevel ? AddIndent(true) : new DumbDisposable();
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

    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, string format, params object[] parameters)
    {
      if (logLevel > _logLevel) return;

      // ReSharper disable once CoVariantArrayConversion
      System.Diagnostics.Trace.WriteLine(GetIndent() + string.Format(format, parameters.Select(ToLogString).ToArray()));
    }

    /// <summary>
    ///   Returns the name of <paramref name="type" /> respecting <see cref="LogFullTypeName" /> property
    /// </summary>
    public static string AsLogString(this Type type) => LogFullTypeName ? type.ToString() : type.GetShortName();

    /// <summary>
    ///   Returns log representation of object, some objects logs in more friendly form then common <see cref="object.ToString" /> returns
    /// </summary>
    public static string ToLogString(object obj) => obj == null ? "null" : AsLogString((dynamic)obj);

    public static void PrintBuildPlans(this BuildPlansCollection buildPlansCollection)
    {
      using (Enabled())
      {
        foreach (var unitSequenceMatcher in buildPlansCollection.Children)
          using (Block(LogLevel.Info, unitSequenceMatcher.ToString()))
          {
            Print(unitSequenceMatcher);
          }
      }
    }

    private static void Print(IUnitSequenceMatcher unitSequenceMatcher)
    {
      try
      {
        foreach (var child in unitSequenceMatcher.Children)
          using (Block(LogLevel.Info, child.ToString()))
          {
            Print(child);
          }
      }
      catch (Exception)
      {
        //ignore
      }
    }

    private static string GetIndent()
    {
      var indent = "";
      for (var i = 0; i < _indent; i++)
        indent += "  ";
      return indent;
    }

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

    private class Indenter : IDisposable
    {
      private readonly int _count;
      private readonly bool _newBlock;

      public Indenter(bool newBlock, int count)
      {
        if (newBlock)
          WriteLine(LogLevel.Info, "{{");

        _newBlock = newBlock;
        _count = count;
        _indent += count;
      }

      public void Dispose()
      {
        _indent -= _count;
        if (_newBlock)
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