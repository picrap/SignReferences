#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName.Project
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class ProjectDefinition: IReferences
    {
        private readonly Microsoft.Build.Evaluation.Project _project;
        private readonly string _projectPath;

        private IEnumerable<AssemblyReference> _references;
        /// <summary>
        /// Gets the references.
        /// </summary>
        /// <value>
        /// The references.
        /// </value>
        public IEnumerable<AssemblyReference> References
        {
            get
            {
                if (_references == null)
                    _references = LoadReferences().ToArray();
                return _references;
            }
        }

        public ProjectDefinition(string path)
        {
            _projectPath = Path.GetDirectoryName(path);
            _project = new Microsoft.Build.Evaluation.Project(path);
        }

        public IEnumerable<AssemblyReference> LoadReferences()
        {
            var references = _project.Items.Where(i => i.ItemType == "Reference").ToArray();
            foreach (var reference in references)
            {
                var hintPath = reference.Metadata.SingleOrDefault(m => m.Name == "HintPath");
                if (hintPath != null)
                {
                    var fullPath = Path.Combine(_projectPath, hintPath.EvaluatedValue);
                    yield return new AssemblyReference(fullPath);
                }
                else
                {
                    var assemblyName = new AssemblyName(reference.EvaluatedInclude);
                    yield return new AssemblyReference(assemblyName);
                }
            }
        }
    }
}
