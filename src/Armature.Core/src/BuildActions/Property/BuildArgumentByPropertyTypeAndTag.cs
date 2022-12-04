using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the property marked with <see cref="InjectAttribute"/> using <see cref="InjectAttribute.Tag"/>
/// as the <see cref="UnitId.Tag"/>
/// </summary>
public record BuildArgumentByPropertyTypeAndTag : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var propertyInfo = (PropertyInfo) buildSession.Stack.TargetUnit.Kind!;

    foreach(var attribute in propertyInfo.GetCustomAttributes<InjectAttribute>())
    {
      Log.WriteLine(LogLevel.Trace, () => $"Attribute: {attribute.ToHoconString()}");

      var unitInfo    = new UnitId(propertyInfo.PropertyType, attribute.Tag);
      var buildResult = buildSession.BuildUnit(unitInfo);

      if(buildResult.HasValue)
      {
        buildSession.BuildResult = buildResult;
        break;
      }
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildArgumentByPropertyTypeAndTag);
}
