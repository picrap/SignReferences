#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

namespace SignReferencesTest
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SignReferences.Signing;

    [TestClass]
    public class AssemblyKeyTest
    {
        [TestMethod]
        public void SimpleAssemblyTest()
        {
            var a = CreateAssemblyKeyProvider();
            var p = a.GetSnkPath("thisOne.dll");
            Assert.AreEqual("some.snk", Path.GetFileName(p));
        }

        [TestMethod]
        public void WildcardAssemblyTest()
        {
            var a = CreateAssemblyKeyProvider();
            var p = a.GetSnkPath("about.dll");
            Assert.AreEqual("other.snk", Path.GetFileName(p));
        }

        private static AssemblyKeyProvider CreateAssemblyKeyProvider()
        {
            var assemblyKeyProvider = new AssemblyKeyProvider(null);
            assemblyKeyProvider.Load(
                @"thisOne.dll: some.snk
a*.dll: other.snk",
                ".");
            return assemblyKeyProvider;
        }
    }
}
