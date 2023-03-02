using JetBrains.Annotations;

namespace Armature.Core;

[PublicAPI]
public static class ExceptionConst
{
  public const string ArmaturePrefix = "Armature_";

  public const string MessageKey = ArmaturePrefix + "Message";
  public const string BuildStack = ArmaturePrefix + "BuildStack";
  public const string Logged  = ArmaturePrefix + "IsLogged";
}