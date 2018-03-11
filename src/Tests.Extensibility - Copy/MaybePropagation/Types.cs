namespace Tests.Extensibility.MaybePropagation
{
  public class Section{}
  
  public interface IReader{
    Section Section { get; }
  }
  public class Reader : IReader
  {
    public Section Section { get; private set; }

    public Reader(Section section) { Section = section; }
  }

  public class Model
  {
    public readonly Maybe<IReader> Reader;

    public Model(Maybe<IReader> reader) { Reader = reader; }
  }
}