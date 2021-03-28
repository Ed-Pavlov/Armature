using System;

namespace Armature.Core.Common
{
  public static class Disposable
  {
    public static IDisposable Create(Action open, Action close) =>
      new Impl<object?>(
        () =>
          {
            open();
            return null;
          },
        _ => close());

    private class Impl<T> : IDisposable
    {
      private readonly Action<T> _close;
      private readonly T _value;

      public Impl(Func<T> open, Action<T> close)
      {
        if (open is null) throw new ArgumentNullException(nameof(open));

        _close = close ?? throw new ArgumentNullException(nameof(close));
        _value = open();
      }

      public void Dispose() => _close(_value);
    }
  }
}