#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

using SignReferences;
using StitcherBoy;

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global
public class SignReferencesTask : StitcherTask<SignReferencesStitcher>
{
    public static int Main(string[] args)
    {
        BlobberHelper.Setup();
        return Run(new SignReferencesTask(), args);
    }
}
