#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Signing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Project;

    /// <summary>
    /// Signs unsigned assemblies
    /// </summary>
    public class AssemblySigner : IDisposable
    {
        private string _defaultKeyFile;

        private string DefaultKeyFile
        {
            get
            {
                if (_defaultKeyFile == null)
                    _defaultKeyFile = CreateSnk(GetTempFileName(".snk"));
                return _defaultKeyFile;
            }
        }

        public void Dispose()
        {
            if (File.Exists(_defaultKeyFile))
                File.Delete(_defaultKeyFile);
        }

        /// <summary>
        /// Signs the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void Sign(AssemblyReference assembly)
        {
            if (assembly.Assembly.GetName().GetPublicKey().Length > 0)
            {
                // resign, which actually never happens :)
                Invoke("sn.exe", "-q -Ra \"{0}\" \"{1}\"", assembly.Path, GetKeyPath(assembly));
            }
            else
            {
                // first sign
                var ilPath = GetTempFileName(".il");
                var isDll = string.Equals(Path.GetExtension(assembly.Path), ".dll", StringComparison.InvariantCultureIgnoreCase);
                var tempOutput = assembly.Path + ".tmp";
                if (File.Exists(tempOutput))
                    File.Delete(tempOutput);
                Invoke("ildasm.exe", "\"{0}\" /text /utf8 /out:\"{1}\"", assembly.Path, ilPath);
                var resFile = Path.Combine(Path.GetDirectoryName(ilPath), Path.GetFileNameWithoutExtension(ilPath) + ".res");
                string includeRes = "";
                if (File.Exists(resFile))
                    includeRes = string.Format("/res=\"{0}\"", resFile);
                Invoke("ilasm.exe", "/quiet \"{0}\" {1} /debug=opt /key=\"{2}\" {4} /output=\"{3}\"", ilPath, isDll ? "/dll" : "/exe", GetKeyPath(assembly), tempOutput, includeRes);
                if (File.Exists(tempOutput))
                {
                    File.Delete(assembly.Path);
                    File.Move(tempOutput, assembly.Path);
                }
            }
        }

        /// <summary>
        /// Gets the name for a temporary file.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <returns></returns>
        private static string GetTempFileName(string suffix)
        {
            var tempDirectory = Path.GetTempPath();
            for (; ; )
            {
                var fileName = Guid.NewGuid().ToString().Substring(0, 8) + suffix;
                var tempPath = Path.Combine(tempDirectory, fileName);
                if (!File.Exists(tempPath))
                    return tempPath;
            }
        }

        /// <summary>
        /// Gets the key path for the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        private string GetKeyPath(AssemblyReference assembly)
        {
            // TODO: refine per assembly (for the ones who care)
            return DefaultKeyFile;
        }

        /// <summary>
        /// Creates the default SNK.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private string CreateSnk(string path, int size = 512)
        {
            Invoke("sn.exe", "-q -k {1} \"{0}\"", path, size);
            return path;
        }

        private void Invoke(string exeName, string commandFormat, params object[] arguments)
        {
            var commandLine = string.Format(commandFormat, arguments);
            var applicationPath = GetApplicationPath(exeName);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(applicationPath, commandLine)
                {
                    UseShellExecute = false
                }
            };

            process.Start();
            process.WaitForExit();
        }

        private readonly IDictionary<string, string> _exePathByName = new Dictionary<string, string>();

        private string GetApplicationPath(string exeName)
        {
            string exePath;
            if (_exePathByName.TryGetValue(exeName, out exePath))
                return exePath;
            _exePathByName[exeName] = exePath = FindApplicationPath(exeName);
            return exePath;
        }

        private static string FindApplicationPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var path = Environment.GetEnvironmentVariable("PATH").Split(';');
            var windowsSdkDir = Environment.GetEnvironmentVariable("WindowsSdkDir");
            return path.Select(p => TryPath(p, fileName)).SingleOrDefault(p => p != null)
                   ?? TryPath(windowsSdkDir, fileName) ?? TryPath(windowsSdkDir, "bin", fileName)
                // I'm not proud of this, but this is for debugging
                   ?? TryPath(@"c:\Windows\Microsoft.NET\Framework\v4.0.30319", fileName)
                   ?? TryPath(@"c:\Windows\Microsoft.NET\Framework\v2.0.50727", fileName)
                   ?? TryPath(@"c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin", fileName)
                   ?? TryPath(@"c:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\Bin", fileName)
                   ?? TryPath(@"c:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\Bin", fileName);
        }

        /// <summary>
        /// Tries the path.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// the path if the file exists, null otherwise
        /// </returns>
        private static string TryPath(string directory, string fileName)
        {
            if (directory == null)
                return null;
            return TryPath(Path.Combine(directory, fileName));
        }

        /// <summary>
        /// Tries the path.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="subDir">The sub dir.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// the path if the file exists, null otherwise
        /// </returns>
        private static string TryPath(string directory, string subDir, string fileName)
        {
            if (directory == null || subDir == null)
                return null;
            return TryPath(Path.Combine(directory, subDir, fileName));
        }

        /// <summary>
        /// Tries the path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns>the path if the file exists, null otherwise</returns>
        private static string TryPath(string fullPath)
        {
            if (File.Exists(fullPath))
                return fullPath;
            return null;
        }
    }
}
