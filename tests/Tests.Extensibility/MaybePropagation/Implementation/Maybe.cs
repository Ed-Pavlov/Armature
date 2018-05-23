namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public class Maybe<T>
  {
    public static readonly Maybe<T> Nothing = new Maybe<T>();

    private Maybe()
    {
    }

    public Maybe(T value)
    {
      Value = value;
      HasValue = true;
    }

    public bool HasValue { get; }
    public T Value { get; }
  }

  public static class Maybe
  {
    public static Maybe<T> ToMaybe<T>(this T value) => new Maybe<T>(value);
  }
}