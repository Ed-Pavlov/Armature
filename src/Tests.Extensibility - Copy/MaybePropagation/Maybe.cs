namespace Tests.Extensibility.MaybePropagation
{
  public class Maybe<T>
  {
    public static readonly Maybe<T> Nothing = new Maybe<T>();
    private readonly bool _hasValue;

    private readonly T _value;

    private Maybe() { }

    public Maybe(T value)
    {
      _value = value;
      _hasValue = true;
    }

    public bool HasValue { get { return _hasValue; } }
    public T Value { get { return _value; } }
  }

  public static class Maybe
  {
    public static Maybe<T> ToMaybe<T>(this T value) { return new Maybe<T>(value); }
  }
}