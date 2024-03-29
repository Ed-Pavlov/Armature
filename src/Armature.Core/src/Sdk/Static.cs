﻿using System.Runtime.CompilerServices;

namespace Armature.Core.Sdk;

public static class Static
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Of<T>() where T : new() => Factory<T>.Instance ??= new T(); // create instance on demand only in order to not spam memory with unused objects

  private static class Factory<T> where T : new()
  {
    public static T? Instance;
  }
}
