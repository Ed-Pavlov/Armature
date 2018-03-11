using System;

// ReSharper disable All

namespace Tests.Functional
{
  public interface IEmptyInterface1
  {
  }

  public interface IEmptyInterface2
  {
  }

  public class EmptyCtorClass : IEmptyInterface1, IEmptyInterface2
  {
    private static int _counter = 1;
    private readonly int _id = _counter++;

    public override string ToString() { return _id.ToString(); }
  }

  public interface IDisposableValue1
  {
    IDisposable Disposable { get; }
  }

  public interface IDisposableValue2
  {
    IDisposable Disposable { get; }
  }

  public class OneDisposableCtorClass : IDisposableValue1, IDisposableValue2
  {
    private readonly IDisposable _disposable;

    public OneDisposableCtorClass(IDisposable disposable) { _disposable = disposable; }

    public IDisposable Disposable { get { return _disposable; } }
  }

  public class OneStringCtorClass : IDisposableValue1, IDisposableValue2
  {
    private readonly string _text;

    public OneStringCtorClass(string text) { _text = text; }

    public string Text { get { return _text; } }

    IDisposable IDisposableValue1.Disposable { get { throw new NotImplementedException(); } }

    IDisposable IDisposableValue2.Disposable { get { throw new NotImplementedException(); } }
  }

  public class TwoDisposableStringCtorClass : OneDisposableCtorClass
  {
    public readonly string String;

    public TwoDisposableStringCtorClass(IDisposable disposable, string @string) : base(disposable) { String = @string; }
  }

  public class Disposable : IDisposable
  {
    public void Dispose() { }
  }
}