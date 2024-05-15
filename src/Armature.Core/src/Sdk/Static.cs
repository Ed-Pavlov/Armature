using System.Runtime.CompilerServices;

namespace BeatyBit.Armature.Core.Sdk;

/// <summary>
/// Generic approach of creating singleton instances of a type when needed.
/// </summary>
public static class Static
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Of<T>() where T : new()
    => Cache<T>.Instance ??= new T(); // create instance on demand only in order to not spam memory with unused objects

  private static class Cache<T> where T : new()
  {
    public static T? Instance;
  }
}