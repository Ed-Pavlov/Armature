using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Properties
{
  public class PropertyByNameMatcher : InjectPointByNameMatcher, IUnitMatcher
  {
    public PropertyByNameMatcher([NotNull] string propertyName) : base(propertyName){}

    protected override string GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as PropertyInfo)?.Name;
  }
}