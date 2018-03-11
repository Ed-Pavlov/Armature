using Armature.Interface;

namespace Tests.Extensibility.MaybePropagation.TestData
{
  public class Section
  {
  }

  public interface IReader
  {
    Section Section { get; }
  }

  public class Reader : IReader
  {
    public Reader(Section section) => Section = section;
    public Section Section { get; }
  }
  
  public class Reader1 : IReader
  {
    public const string InjectPointId = "SectionType";
    
    public Reader1([Inject(InjectPointId)]Section section) => Section = section;
    public Section Section { get; }
  }
}