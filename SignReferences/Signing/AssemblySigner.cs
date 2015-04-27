#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Signing
{
    using System;
    using System.IO;
    using Project;
    using Utility;

    /// <summary>
    /// Signs unsigned assemblies
    /// </summary>
    public class AssemblySigner
    {
        /// <summary>
        /// Signs the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="snkPath">The SNK path.</param>
        public void Sign(AssemblyReference assembly, string snkPath)
        {
            if (assembly.Assembly.GetName().GetPublicKey().Length > 0)
            {
                // resign, which actually never happens :)
                ProcessUtility.Invoke("sn.exe", "-q -Ra \"{0}\" \"{1}\"", assembly.Path, snkPath);
            }
            else
            {
                // first sign
                var ilPath = PathUtility.GetTempFileName(".il");
                var isDll = string.Equals(Path.GetExtension(assembly.Path), ".dll", StringComparison.InvariantCultureIgnoreCase);
                var tempOutput = assembly.Path + ".tmp";
                if (File.Exists(tempOutput))
                    File.Delete(tempOutput);
                ProcessUtility.Invoke("ildasm.exe", "\"{0}\" /text /utf8 /out:\"{1}\"", assembly.Path, ilPath);
                var resFile = Path.Combine(Path.GetDirectoryName(ilPath), Path.GetFileNameWithoutExtension(ilPath) + ".res");
                string includeRes = "";
                if (File.Exists(resFile))
                    includeRes = string.Format("/res=\"{0}\"", resFile);
                ProcessUtility.Invoke("ilasm.exe", "/quiet \"{0}\" {1} /debug=opt /key=\"{2}\" {4} /output=\"{3}\"", ilPath, isDll ? "/dll" : "/exe", snkPath, tempOutput, includeRes);
                if (File.Exists(tempOutput))
                {
                    File.Delete(assembly.Path);
                    File.Move(tempOutput, assembly.Path);
                }
            }
        }
    }
}
