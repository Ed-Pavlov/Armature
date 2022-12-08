using System.Runtime.CompilerServices;

namespace Armature.Core.Sdk;

public static class ExtensibilityExtension
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IInternal<T> GetInternals<T>(this IInternal<T> obj) => obj;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IInternal<T1, T2> GetInternals<T1, T2>(this IInternal<T1, T2> obj) => obj;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IInternal<T1, T2, T3> GetInternals<T1, T2, T3>(this IInternal<T1, T2, T3> obj) => obj;
}

/// <summary>
/// This interface provide access to the internal fields in the generic form for the sake of possible extensibility by the end user.
/// See implementation for details.
/// </summary>
public interface IInternal<out T1>
{
  T1 Member1 { get; }
}

///<inheritdoc />
public interface IInternal<out T1, out T2> : IInternal<T1>
{
  T2 Member2 { get; }
}

///<inheritdoc />
public interface IInternal<out T1, out T2, out T3> : IInternal<T1, T2>
{
  T3 Member3 { get; }
}
