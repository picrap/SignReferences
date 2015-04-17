#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

using System.Diagnostics;
using System.IO;
using CommandLine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SignReferences;
using SignReferences.Logging;
using SignReferences.Signing;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once CheckNamespace
public class SignUnsignedReferences : Task
{
    /// <summary>
    /// Gets or sets the project path.
    /// </summary>
    /// <value>
    /// The project path.
    /// </value>
    [Required]
    public string ProjectPath { get; set; }

    /// <summary>
    /// Gets the wrapped task path.
    /// </summary>
    /// <returns></returns>
    private string GetWrappedTaskPath()
    {
        var thisPath = GetType().Assembly.Location;
        var wrappedTaskPath = Path.Combine(Path.GetDirectoryName(thisPath), Path.GetFileNameWithoutExtension(thisPath));
        if (File.Exists(wrappedTaskPath))
            return wrappedTaskPath;
        return null;
    }

    /// <summary>
    /// Target task entry point
    /// </summary>
    /// <returns>
    /// true for success
    /// </returns>
    public override bool Execute()
    {
        var wrappedTaskPath = GetWrappedTaskPath();
        var logging = new TaskLogging(this);
        if (wrappedTaskPath == null)
        {
            var projectSigner = new ProjectSigner(logging);
            projectSigner.Sign(ProjectPath);
        }
        else
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = wrappedTaskPath,
                    Arguments = string.Format("--{0}=\"{1}\"", Parameters.ProjectOption, ProjectPath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                }
            };
            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    logging.Write(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
        return true;
    }

    /// <summary>
    /// Command-line entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        var parameters = Parser.Default.ParseArguments<Parameters>(args);
        var projectSigner = new ProjectSigner(new ConsoleLogging());
        projectSigner.Sign(parameters.Value.ProjectPath);
    }
}
