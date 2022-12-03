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
/// I would like to make all the "internal" details of framework parts accessible for sake of extensibility. If anyone wants to extends this framework
/// they should be able to do it. The most simple way is to make fields and methods public but this will lead that IDE intellisense will be
/// "polluted" with all these internal members.
///
/// This interface provide access to the internal fields in the generic form, see implementation for details.
/// </summary>
public interface IInternal<out T1>
{
  T1 Member1 { get; }
}

public interface IInternal<out T1, out T2> : IInternal<T1>
{
  T2 Member2 { get; }
}

public interface IInternal<out T1, out T2, out T3> : IInternal<T1, T2>
{
  T3 Member3 { get; }
}