#region AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

using System.Reflection;
using System.Runtime.InteropServices;

// --- Product

[assembly: AssemblyCompany("craponne.fr ;)")]
[assembly: AssemblyProduct("AutoStrongName")]

[assembly: AssemblyCopyright("Distributed under MIT license http://opensource.org/licenses/MIT")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.1")]

// --- Product

[assembly: AssemblyTitle("AutoStrongName")]
[assembly: AssemblyDescription("Signs unsigned assemblies at build-time. This is useful if your project requires signing and you use third-party unsigned libraries.")]

[assembly: ComVisible(false)]

