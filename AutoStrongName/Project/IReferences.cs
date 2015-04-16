#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName.Project
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
