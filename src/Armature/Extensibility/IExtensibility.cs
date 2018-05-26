namespace Armature.Extensibility
{
  public interface IExtensibility<out T1>
  {
    T1 Item1 { get; }
  }
  
  public interface IExtensibility<out T1, out T2>
  {
    T1 Item1 { get; }
    T2 Item2 { get; }
  }
}