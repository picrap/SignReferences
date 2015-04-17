#region SignReferences
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/SignReferences
#endregion
namespace SignReferences.Logging
{
    using System;

    /// <summary>
    /// Very basic console logging
    /// </summary>
    public class ConsoleLogging : ILogging
    {
        public void Write(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
        }

        public void WriteWarning(string format, params object[] parameters)
        {
            Write("! " + format, parameters);
        }

        public void WriteError(string format, params object[] parameters)
        {
            Write("* " + format, parameters);
        }

        public void WriteDebug(string format, params object[] parameters)
        {
#if DEBUG
            Write(". " + format, parameters);
#endif
        }
    }
}