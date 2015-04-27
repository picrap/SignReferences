using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignReferences.Utility
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Utility for processes
    /// </summary>
    public static class ProcessUtility
    {
        /// <summary>
        /// Invokes the specified executable.
        /// </summary>
        /// <param name="exeName">Name of the executable.</param>
        /// <param name="commandFormat">The command format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void Invoke(string exeName, string commandFormat, params object[] arguments)
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

        private static readonly IDictionary<string, string> ExePathByName = new Dictionary<string, string>();

        private static string GetApplicationPath(string exeName)
        {
            string exePath;
            if (ExePathByName.TryGetValue(exeName, out exePath))
                return exePath;
            ExePathByName[exeName] = exePath = FindApplicationPath(exeName);
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
