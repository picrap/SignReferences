#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion

namespace SignReferences.Utility
{
    using System;
    using System.IO;

    /// <summary>
    /// Utility for path
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// Gets the name for a temporary file.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <returns></returns>
        public static string GetTempFileName(string suffix)
        {
            var tempDirectory = Path.GetTempPath();
            for (; ; )
            {
                var fileName = Guid.NewGuid().ToString().Substring(0, 8) + suffix;
                var tempPath = Path.Combine(tempDirectory, fileName);
                if (!File.Exists(tempPath))
                    return tempPath;
            }
        }
    }
}
