﻿using JetBrains.Annotations;

namespace Armature.Core.Sdk;

[PublicAPI]
public static class ExceptionConst
{
  public const string ArmaturePrefix = "Armature_";

  public const string Context = ArmaturePrefix + "Context";
  public const string Logged  = ArmaturePrefix + "Logged";
}