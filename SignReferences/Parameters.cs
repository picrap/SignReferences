#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences
{
    using CommandLine;

    /// <summary>
    /// Application parameters
    /// </summary>
    public class Parameters
    {
        public const string ProjectOption = "project";
        public const string SolutionOption = "solution";

        /// <summary>
        /// Gets or sets the project path.
        /// </summary>
        /// <value>
        /// The project path.
        /// </value>
        [Option('p', ProjectOption, Required = true, HelpText = "Project path.")]
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the solution path.
        /// </summary>
        /// <value>
        /// The solution path.
        /// </value>
        [Option('s', SolutionOption, Required = false, HelpText = "Solution path.")]
        public string SolutionPath { get; set; }
    }
}
