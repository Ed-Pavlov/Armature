using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Injects values into building Unit properties specified for injection
  /// </summary>
  public record InjectDependenciesIntoProperties : IBuildAction
  {
    public static readonly IBuildAction Instance = new InjectDependenciesIntoProperties();

    private InjectDependenciesIntoProperties() { }

    [DebuggerStepThrough]
    public void Process(IBuildSession buildSession) { }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
      {
        var unit = buildSession.BuildResult.Value;
        var type = unit!.GetType();

        var unitInfo = new UnitId(type, SpecialKey.PropertiesList);
        var unitList = buildSession.BuildAllUnits(unitInfo).Select(_ => _.Entity); //TODO: do we need to take into account weight of matching? 

        foreach(var property in unitList.Select(result => result.Value!).SelectMany(list => (PropertyInfo[])list))
        {
          // var property = (PropertyInfo)buildResult.Value!;

          var argument = buildSession.BuildPropertyArgument(property);
          property.SetValue(unit, argument);
        }
      }
    }
  }
}
