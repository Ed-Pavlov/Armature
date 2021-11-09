using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Armature.Core.build;

[assembly: AssemblyTitle("Armature.Core")]
[assembly: AssemblyDescription("The core part of Armature, can be used to make another frameworks")]

[assembly: InternalsVisibleTo("Tests, PublicKey="    + Public.Key)]
// [assembly: InternalsVisibleTo("Armature, PublicKey=" + Public.Key)]

namespace Armature.Core.build
{
  [SuppressMessage("ReSharper", "CheckNamespace")]
  internal static class Public
  {
    public const string Key =
      "0024000004800000940000000602000000240000525341310004000001000100e521fa7f778455d9"
    + "d6ffd544b626b77a9ba302a23aec6da8ba9dac0ff274af79f1bc4d2b7c1fae922b2c971f933ed037"
    + "211c012ddc09b4d1da193d072fe55ae950f3a6a215cbe76ace833145f65a6980cda1ae1ca492e2d5"
    + "304b2197c86a0f750ec7b52176f2b3aa2a26a327a19dd93aa2f72124e728a82f774240a351f8ddb4";
  }
}

namespace System.Runtime.CompilerServices
{
  internal class IsExternalInit { }
}