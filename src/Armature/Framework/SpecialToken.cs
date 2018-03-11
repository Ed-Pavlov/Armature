using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  /// <summary>
  ///   These tokens are used by Armature framework to distinguish internal build steps from possible users for the same types
  /// </summary>
  public static class SpecialToken
  {
    /// <summary>
    ///   Is used to build a value for parameter
    /// </summary>
    public static readonly object ParameterValue = new Token("ParameterValue");

    /// <summary>
    ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
    /// </summary>
    public static readonly object Constructor = new Token("Constructor");
  }
}