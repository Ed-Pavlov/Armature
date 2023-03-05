using System.Reflection;
using System.Runtime.CompilerServices;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Sdk;

/// <summary>
/// These tags are used by Armature to build such units as a constructor needed to instantiate an object, or an argument for the method parameter and so on.
///
/// If you need to extend the set of special tags with your own, make a derived class and create tags using protected constructor.
/// </summary>
public class ServiceTag : Tag
{
  [PublicAPI]
  public ServiceTag([CallerMemberName] string name = "") : base(name) { }

  /// <summary>
  /// Is used to "build" a <see cref="ConstructorInfo" /> for a type
  /// </summary>
  public static readonly ServiceTag  Constructor = new();
  /// <summary>
  /// Is used to build a collection of properties of a type
  /// </summary>
  public static readonly ServiceTag  PropertyCollection = new();
  /// <summary>
  /// Is used to build an argument for the inject point
  /// </summary>
  public static readonly ServiceTag  Argument = new ();
}