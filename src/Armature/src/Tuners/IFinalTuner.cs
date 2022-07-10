namespace Armature;

public interface IFinalTuner : IDependencyTuner<IFinalAndContextTuner>
{
  IContextTuner AsSingleton();
}
