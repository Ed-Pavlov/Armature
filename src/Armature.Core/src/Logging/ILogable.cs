namespace Armature.Core.Logging
{
  public interface ILogable : ILogable1
  {
    void   PrintToLog();
  }

  public interface ILogable1
  {
    string ToLogString();
  }
}
