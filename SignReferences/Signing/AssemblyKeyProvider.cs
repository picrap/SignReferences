#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

namespace SignReferences.Signing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Utility;
    using WildcardMatch;

    /// <summary>
    /// Key provider provides keys
    /// </summary>
    public class AssemblyKeyProvider : IDisposable
    {
        private string _defaultKeyFile;

        /// <summary>
        /// Gets the default key file.
        /// </summary>
        /// <value>
        /// The default key file.
        /// </value>
        private string DefaultKeyFile
        {
            get
            {
                if (_defaultKeyFile == null)
                    _defaultKeyFile = CreateSnk(PathUtility.GetTempFileName(".snk"));
                return _defaultKeyFile;
            }
        }

        private readonly List<AssemblyKey> _assemblyKeys = new List<AssemblyKey>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyKeyProvider" /> class.
        /// </summary>
        /// <param name="relatedFilePaths">The related file paths.</param>
        public AssemblyKeyProvider(params string[] relatedFilePaths)
        {
            foreach (var relatedFilePath in relatedFilePaths)
                Load(relatedFilePath);
        }

        /// <summary>
        /// Loads the reference file beside the given file.
        /// </summary>
        /// <param name="relatedFilePath">The related file path.</param>
        private void Load(string relatedFilePath)
        {
            if (relatedFilePath == null)
                return;
            var directory = Path.GetDirectoryName(relatedFilePath);
            var referencesTxt = Path.Combine(directory, "SignReferences.txt");
            if (File.Exists(referencesTxt))
                Load(File.ReadAllText(referencesTxt), directory);
        }

        /// <summary>
        /// Loads the specified text data.
        /// </summary>
        /// <param name="referencesText">The yaml data.</param>
        /// <param name="projectDir">The project dir.</param>
        public void Load(string referencesText, string projectDir)
        {
            var keys = new List<AssemblyKey>();
            using (var stringReader = new StringReader(referencesText))
            {
                for (;;)
                {
                    var line = stringReader.ReadLine();
                    if (line == null)
                        break;
                    var colonIndex = line.IndexOf(':');
                    if (colonIndex < 0)
                        continue;

                    var assemblyMask = line.Substring(0, colonIndex).Trim();
                    var snkPath = Path.Combine(projectDir, line.Substring(colonIndex + 1).Trim());
                    keys.Add(new AssemblyKey(assemblyMask, snkPath));
                }
            }
            _assemblyKeys.AddRange(keys);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if (File.Exists(_defaultKeyFile))
                File.Delete(_defaultKeyFile);
        }

        /// <summary>
        /// Gets the SNK path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns></returns>
        public string GetSnkPath(string assemblyPath)
        {
            // find a matching assembly
            var assemblyFileName = Path.GetFileName(assemblyPath);
            var matchingAssemblyKey = _assemblyKeys.FirstOrDefault(a => a.Name.WildcardMatch(assemblyFileName, true));
            if (matchingAssemblyKey != null)
                return matchingAssemblyKey.SnkPath;
            // otherwise, use default
            return DefaultKeyFile;
        }

        /// <summary>
        /// Creates the default SNK.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private static string CreateSnk(string path, int size = 512)
        {
            ProcessUtility.Invoke("sn.exe", "-q -k {1} \"{0}\"", path, size);
            return path;
        }
    }
}
