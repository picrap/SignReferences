﻿#region SignReferences
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
            var publicKey = assembly.Assembly.GetName().GetPublicKey();
            // check the referenced assembly is not already signed, and then, sign it!
            if (publicKey.Length == 0)
            {
                // first sign
                // no simple solution found here, so here is one way to do it:
                // 1. disassemble to .il (and optionally .res) files, using ildasm
                // 2. reassemble file, with given snk path, using ilasm
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
