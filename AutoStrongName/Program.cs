#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName
{
    using System.Linq;
    using Build;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var project = new Project(@"..\..\AutoStrongName.csproj");
            var references = project.GetReferences().ToArray();
            foreach (var r in references)
            {
                var a = r.Resolve();
                if (a.GlobalAssemblyCache)
                    continue;
                var t = a.GetName().GetPublicKey();
            }
        }
    }
}
