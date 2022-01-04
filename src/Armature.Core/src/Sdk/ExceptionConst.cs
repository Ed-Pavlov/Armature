using JetBrains.Annotations;

namespace Armature.Core.Sdk;

[PublicAPI]
public static class ExceptionConst
{
  public const string ArmaturePrefix = "Armature_";

  public const string BuildChain = ArmaturePrefix + "BuildChain";
  public const string Logged  = ArmaturePrefix + "IsLogged";
}