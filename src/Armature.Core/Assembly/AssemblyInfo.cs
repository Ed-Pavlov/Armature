using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Armature.Core")]
[assembly: AssemblyDescription("Core part of Armature, can be used to make another frameworks")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("44b349af-23a3-4a84-b397-f84ec58e4518")]

#if DEBUG
[assembly: InternalsVisibleTo("Tests")]
[assembly: InternalsVisibleTo("Armature")]
#else
[assembly: InternalsVisibleTo("Tests")]
[assembly: InternalsVisibleTo("Armature")]
//[assembly: InternalsVisibleTo("Tests, PublicKey=" + Public.Key)]
//[assembly: InternalsVisibleTo("Armature, PublicKey=" + Public.Key)]
#endif


static class Public
{
  public const string Key = 
    "00240000048000009400000006020000002400005253413100040000010001002da2d8f192cad5f3" +
    "5f9a09b0fde397d46cb0839e633acb05d9b23888b9db6d90c559b85406e53c9c8b71499db5428bc1" +
    "0890b38688c61407b202603bcf4077b8616a334fdb2d3b62cec0417a8a96e32c92b4565d1347fa88" +
    "ca02dd134dce1d7bdad9817b3a856153d2bc53c37bbae5a8aa2765c96d5017517e1f0a8c1458aed5";
} 