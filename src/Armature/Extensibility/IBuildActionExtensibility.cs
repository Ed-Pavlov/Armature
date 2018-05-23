using Armature.Core;

namespace Armature.Extensibility
{
  internal interface IBuildActionExtensibility : IUnitMatcherExtensibility
  {
    IBuildAction BuildAction { get; }
  }
}