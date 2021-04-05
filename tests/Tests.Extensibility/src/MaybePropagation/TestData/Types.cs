using Armature;

namespace Tests.Extensibility.MaybePropagation.TestData
{
  internal class Section { }

  internal interface IReader
  {
    Section Section { get; }
  }

  internal class Reader : IReader
  {
    public Reader(Section section) => Section = section;
    public Section Section { get; }
  }

  internal class Reader1 : IReader
  {
    public const string InjectPointId = "SectionType";

    public Reader1([Inject(InjectPointId)] Section section) => Section = section;
    public Section Section { get; }
  }
}
