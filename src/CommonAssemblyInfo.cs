using System;
using System.Reflection;

#pragma warning disable 7035

[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyProduct("Armature")]
[assembly: AssemblyCopyright("Copyright © 2018 Ed Pavlov")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion(Product.Version)]
[assembly: AssemblyInformationalVersion(Product.Version)]
[assembly: AssemblyFileVersion(Product.Version)]

internal static class Product
{
  public const string Version = "1.0.8";
}
