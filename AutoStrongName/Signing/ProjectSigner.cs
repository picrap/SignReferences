namespace AutoStrongName.Signing
{
    using System.Linq;
    using Logging;
    using Project;

    /// <summary>
    /// Class to autosign project
    /// </summary>
    public class ProjectSigner
    {
        private readonly ILogging _logging;

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
            _logging.Write("Checking project {0}", projectPath);
            var project = new ProjectDefinition(projectPath);
            var unsignedReferences = project.GetReferences(r => !(r.IsSigned ?? true)).ToArray();
            using (var assemblySigner = new AssemblySigner())
            {
                foreach (var unsignedReference in unsignedReferences)
                {
                    _logging.Write("Signed Assembly {0}", unsignedReference.AssemblyName.FullName);
                    assemblySigner.Sign(unsignedReference);
                }
            }
        }
    }
}
