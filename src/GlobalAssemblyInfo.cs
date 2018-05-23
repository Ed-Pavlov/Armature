using System;
using System.Reflection;

#pragma warning disable 7035

[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
[assembly:AssemblyKeyFile(@"..\..\private\armature.snk")]
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
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
