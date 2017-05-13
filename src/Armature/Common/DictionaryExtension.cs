using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Armature.Common
{
  public static class DictionaryExtension
  {
    public static TValue GetOrCreateValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createValue)
    {
      TValue value;
      if (!dictionary.TryGetValue(key, out value))
      {
        value = createValue();
        dictionary.Add(key, value);
      }
      return value;
    }

    [CanBeNull]
    public static TValue GetValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    [CanBeNull]
    public static TValue GetValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createValue)
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : createValue();
    }
  }
}