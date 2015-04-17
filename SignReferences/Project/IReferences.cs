#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Project
{
    using System.Collections.Generic;

    public  interface IReferences
    {
        /// <summary>
        /// Gets the references.
        /// </summary>
        /// <value>
        /// The references.
        /// </value>
        IEnumerable<AssemblyReference> References { get; }
    }
}
