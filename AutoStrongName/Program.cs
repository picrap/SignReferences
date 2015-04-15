#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName
{
    using System.Linq;
    using Build;
    using Utility;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var project = new Project(@"..\..\AutoStrongName.csproj");
            var unsignedReferences = project.GetReferences(r => !(r.IsSigned ?? true)).ToArray();
            using (var assemblySigner = new AssemblySigner())
            {
                unsignedReferences.ForAll(assemblySigner.Sign);
            }
        }
    }
}
