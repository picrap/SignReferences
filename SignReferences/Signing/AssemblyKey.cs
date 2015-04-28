#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

namespace SignReferences.Signing
{
    public class AssemblyKey
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        public string SnkPath { get; private set; }

        /// <summary>
        /// Assemblies the key.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="snkPath">The SNK path.</param>
        /// <returns></returns>
        public AssemblyKey(string name, string snkPath)
        {
            Name = name;
            SnkPath = snkPath;
        }
    }
}