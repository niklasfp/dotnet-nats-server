using System.Runtime.InteropServices;

namespace NatsServer.Net
{
    internal static class NatsServerLib
    {
        const string LibName = "nats-serverlib";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallback([In][MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShutdownServer();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint StartServer([MarshalAs(UnmanagedType.LPStr)] string configFile, LogCallback callback);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterLogger(LogCallback callback);
    }
}