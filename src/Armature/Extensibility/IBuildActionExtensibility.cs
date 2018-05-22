using Armature.Core;

namespace Armature.Extensibility
{
  interface IBuildActionExtensibility : IUnitMatcherExtensibility
  {
    IBuildAction BuildAction { get; }
  }
}