#region SignReferences

// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences

#endregion

namespace SignReferences
{
    using System;
    using System.Linq;
    using Signing;
    using StitcherBoy.Reflection;
    using StitcherBoy.Weaving.Build;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class SignReferencesStitcher : AssemblyStitcher
    {
        protected override bool Process(AssemblyStitcherContext context)
        {
            var assemblyKeyProvider = new AssemblyKeyProvider(".");
            foreach (var dependency in context.Dependencies.Where(d => d.IsPrivate))
                Sign(dependency, assemblyKeyProvider);
            return false; // whatever, actually, because we didn't load the main assembly (which at this stage is not even built)
        }

        private void Sign(AssemblyDependency dependency, AssemblyKeyProvider assemblyKeyProvider)
        {
            using (var moduleHandler = new ModuleManager(dependency.Path, false))
            {
                if (moduleHandler.Module.IsStrongNameSigned)
                    return;

                var snkPath = assemblyKeyProvider.GetSnkPath(dependency.Path);
                moduleHandler.Write(snkPath);
                Logging.Write("Signed assembly {0}", moduleHandler.Module.FullName);
            }
        }
    }
}
