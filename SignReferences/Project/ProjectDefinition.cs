﻿#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Project
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Wraps a project (from Microsoft.Build)
    /// </summary>
    public class ProjectDefinition : IReferences
    {
        private readonly Project _project;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDefinition"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ProjectDefinition(string path)
        {
            _projectPath = Path.GetDirectoryName(path);
            using (var projectReader = File.OpenText(path))
            using (var xmlReader = new XmlTextReader(projectReader))
                _project = new Project(xmlReader);
        }

        /// <summary>
        /// Loads the references.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<AssemblyReference> LoadReferences()
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
