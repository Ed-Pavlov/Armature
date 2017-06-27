using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Armature.Common
{
  internal static class DictionaryExtension
  {
    public static TValue GetOrCreateValue<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull] Func<TValue> createValue)
    {
      if (dictionary == null) throw new ArgumentNullException("dictionary");
      if (key == null) throw new ArgumentNullException("key");
      if (createValue == null) throw new ArgumentNullException("createValue");
      
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
  }
}