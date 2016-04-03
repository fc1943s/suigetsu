using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Suigetsu.Core.Desktop.Ext
{
    [ExcludeFromCodeCoverage]
    public static class NativeMethods
    {
        [Flags]
        public enum ScmAccessRights
        {
            Connect = 0x0001,
            CreateService = 0x0002,
            EnumerateService = 0x0004,
            Lock = 0x0008,
            QueryLockStatus = 0x0010,
            ModifyBootConfig = 0x0020,
            StandardRightsRequired = 0xF0000,

            AllAccess =
                StandardRightsRequired | Connect | CreateService | EnumerateService | Lock | QueryLockStatus | ModifyBootConfig
        }

        [Flags, SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum ServiceAccessRights
        {
            QueryConfig = 0x1,
            ChangeConfig = 0x2,
            QueryStatus = 0x4,
            EnumerateDependants = 0x8,
            Start = 0x10,
            Stop = 0x20,
            PauseContinue = 0x40,
            Interrogate = 0x80,
            UserDefinedControl = 0x100,
            Delete = 0x00010000,
            StandardRightsRequired = 0xF0000,

            AllAccess =
                StandardRightsRequired | QueryConfig | ChangeConfig | QueryStatus | EnumerateDependants | Start | Stop
                | PauseContinue | Interrogate | UserDefinedControl
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum ServiceBootFlag
        {
            Start = 0x00000000,
            SystemStart = 0x00000001,
            AutoStart = 0x00000002,
            DemandStart = 0x00000003,
            Disabled = 0x00000004
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum ServiceControl
        {
            Stop = 0x00000001,
            Pause = 0x00000002,
            Continue = 0x00000003,
            Interrogate = 0x00000004,
            Shutdown = 0x00000005,
            ParamChange = 0x00000006,
            NetBindAdd = 0x00000007,
            NetBindRemove = 0x00000008,
            NetBindEnable = 0x00000009,
            NetBindDisable = 0x0000000A
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum ServiceError
        {
            Ignore = 0x00000000,
            Normal = 0x00000001,
            Severe = 0x00000002,
            Critical = 0x00000003
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum ServiceState
        {
            Unknown = -1,
            NotFound = 0,
            Stopped = 1,
            StartPending = 2,
            StopPending = 3,
            Running = 4,
            ContinuePending = 5,
            PausePending = 6,
            Paused = 7
        }

        public const int ServiceWin32OwnProcess = 0x00000010;
        public const int ServiceConfigDescription = 0x01;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ServiceDescription
        {
            public string lpDescription;
        }

        [StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class ServiceStatus
        {
            public int dwServiceType = 0;
            public ServiceState dwCurrentState = 0;
            public int dwControlsAccepted = 0;
            public int dwWin32ExitCode = 0;
            public int dwServiceSpecificExitCode = 0;
            public int dwCheckPoint = 0;
            public int dwWaitHint = 0;
        }

        #region advapi32.dll

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, IntPtr lpInfo);

        [DllImport("advapi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr OpenSCManagerW(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenServiceW(IntPtr hScManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateServiceW
            (IntPtr hScManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess,
             int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName,
             string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseServiceHandle(IntPtr hScObject);

        [DllImport("advapi32.dll")]
        internal static extern int QueryServiceStatus(IntPtr hService, ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll")]
        internal static extern int ControlService(IntPtr hService, ServiceControl dwControl, ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int StartService(IntPtr hService, int dwNumServiceArgs, IntPtr lpServiceArgVectors);

        #endregion
    }
}
