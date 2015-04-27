#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Signing
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Logging;
    using Project;
    using Utility;

    /// <summary>
    /// Class to autosign project
    /// </summary>
    public class ProjectSigner : IDisposable
    {
        private readonly ILogging _logging;

        private string _defaultKeyFile;

        private string DefaultKeyFile
        {
            get
            {
                if (_defaultKeyFile == null)
                    _defaultKeyFile = CreateSnk(PathUtility.GetTempFileName(".snk"));
                return _defaultKeyFile;
            }
        }

        public void Dispose()
        {
            if (File.Exists(_defaultKeyFile))
                File.Delete(_defaultKeyFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSigner"/> class.
        /// </summary>
        /// <param name="logging">The logging.</param>
        public ProjectSigner(ILogging logging)
        {
            _logging = logging;
        }

        /// <summary>
        /// Signs the project specified by full path.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        public void Sign(string projectPath)
        {
            var stopwatch = new Stopwatch();
            var project = new ProjectDefinition(projectPath);
            var unsignedReferences = project.GetReferences(r => !(r.IsSigned ?? true)).ToArray();
            var assemblySigner = new AssemblySigner();
            foreach (var unsignedReference in unsignedReferences)
            {
                _logging.Write("SignReference signed Assembly {0}", unsignedReference.AssemblyName.FullName);
                assemblySigner.Sign(unsignedReference, GetKeyPath(unsignedReference));
            }
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            _logging.WriteDebug("SignReferences ran in {0}ms", elapsedMilliseconds);
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
            ProcessUtility.Invoke("sn.exe", "-q -k {1} \"{0}\"", path, size);
            return path;
        }
    }
}
