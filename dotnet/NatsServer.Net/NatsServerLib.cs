﻿using System.Runtime.InteropServices;

namespace NatsServer.Net
{
    internal static class NatsServerLib
    {
        const string LibName = "nats-serverlib";
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallback([MarshalAs(UnmanagedType.LPUTF8Str)] string message, long level);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint CreateServer([MarshalAs(UnmanagedType.LPStr)] string arguments, LogCallback callback);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ShutdownServer(nuint handle);
        
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeServer(nuint handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint StartServer(nuint handle);
    }
}