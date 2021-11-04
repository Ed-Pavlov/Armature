using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Injects values into building Unit properties specified for injection
  /// </summary>
  public record InjectDependenciesIntoProperties : IBuildAction
  {
    [DebuggerStepThrough]
    public void Process(IBuildSession buildSession) { }

    public void PostProcess(IBuildSession buildSession)
    {
      var unit = buildSession.BuildResult.Value;
      var type = buildSession.GetUnitUnderConstruction().GetUnitTypeSafe(); // ?? unit!.GetType(); //TODO: is it good implementation?

      var unitInfo = new UnitId(type, SpecialKey.PropertyList);
      var unitList = buildSession.BuildAllUnits(unitInfo).Select(_ => _.Entity); //TODO: do we need to take into account weight of matching?

      foreach(var property in unitList.Select(result => result.Value!).SelectMany(list => (PropertyInfo[])list))
      {
        var argument = buildSession.BuildPropertyArgument(property);
        property.SetValue(unit, argument);
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => nameof(InjectDependenciesIntoProperties);
  }
}