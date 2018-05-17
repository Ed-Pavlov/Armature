using Armature;

namespace Tests.Extensibility.MaybePropagation.TestData
{
  class Section
  {
  }

  interface IReader
  {
    Section Section { get; }
  }

  class Reader : IReader
  {
    public Reader(Section section) => Section = section;
    public Section Section { get; }
  }
  
  class Reader1 : IReader
  {
    public const string InjectPointId = "SectionType";
    
    public Reader1([Inject(InjectPointId)]Section section) => Section = section;
    public Section Section { get; }
  }
}