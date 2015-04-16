#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName.Logging
{
    using System.Diagnostics;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// ILogging implementation for Task
    /// </summary>
    public class TaskLogging : ILogging
    {
        private readonly TaskLoggingHelper _logging;

        public TaskLogging(ITask task)
        {
            _logging = new TaskLoggingHelper(task);
        }

        public void Write(string format, params object[] parameters)
        {
            _logging.LogMessage(MessageImportance.High, format, parameters);
        }

        public void WriteWarning(string format, params object[] parameters)
        {
            _logging.LogWarning(format, parameters);
        }

        public void WriteError(string format, params object[] parameters)
        {
            _logging.LogError(format, parameters);
        }

        public void WriteDebug(string format, params object[] parameters)
        {
#if DEBUG
            _logging.LogMessage(MessageImportance.High, format, parameters);
#endif
        }
    }
}
