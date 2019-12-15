using System.Runtime.InteropServices;

namespace YtbDownloader.Core.Win32
{
    internal enum CtrlTypes : uint
    {
        CTRL_C_EVENT,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    internal delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

    internal class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll")]
        internal static extern bool FreeConsole();
    }
}