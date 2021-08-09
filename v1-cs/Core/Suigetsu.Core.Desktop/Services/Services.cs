using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Suigetsu.Core.Desktop.Ext;
using Suigetsu.Core.Desktop.Util;

namespace Suigetsu.Core.Desktop.Services
{
    [ExcludeFromCodeCoverage]
    public static class Services
    {
        public static void Uninstall(string serviceName)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.AllAccess);

            try
            {
                var service = NativeMethods.OpenServiceW(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);
                if(service == IntPtr.Zero)
                {
                    throw new ServiceException("Service not installed.");
                }

                try
                {
                    if(GetServiceStatus(service) == NativeMethods.ServiceState.Running)
                    {
                        StopService(service);
                    }
                    if(!NativeMethods.DeleteService(service))
                    {
                        throw new ServiceException("Could not delete service " + Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        public static bool ServiceIsInstalled(string serviceName)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.Connect);

            try
            {
                var service = NativeMethods.OpenServiceW(scm, serviceName, NativeMethods.ServiceAccessRights.QueryStatus);

                if(service == IntPtr.Zero)
                {
                    return false;
                }

                NativeMethods.CloseServiceHandle(service);
                return true;
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        public static void Install
            (string serviceName, string displayName, string fileName, string description = "", bool start = false)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.AllAccess);

            try
            {
                var service = IntPtr.Zero;
                try
                {
                    service = NativeMethods.OpenServiceW(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);

                    if(service == IntPtr.Zero)
                    {
                        service = NativeMethods.CreateServiceW
                            (scm,
                             serviceName,
                             displayName,
                             NativeMethods.ServiceAccessRights.AllAccess,
                             NativeMethods.ServiceWin32OwnProcess,
                             NativeMethods.ServiceBootFlag.AutoStart,
                             NativeMethods.ServiceError.Normal,
                             fileName,
                             null,
                             IntPtr.Zero,
                             null,
                             null,
                             null);
                    }
                    else
                    {
                        return;
                    }

                    if(service == IntPtr.Zero)
                    {
                        throw new ServiceException("Failed to install service.");
                    }

                    if(description != string.Empty)
                    {
                        var serviceDescription = new NativeMethods.ServiceDescription
                        {
                            lpDescription = description
                        };

                        var lpInfo = Marshal.AllocHGlobal(Marshal.SizeOf(serviceDescription));

                        Marshal.StructureToPtr(serviceDescription, lpInfo, false);

                        NativeMethods.ChangeServiceConfig2(service, NativeMethods.ServiceConfigDescription, lpInfo);

                        Marshal.FreeHGlobal(lpInfo);
                    }

                    if(start)
                    {
                        StartService(service);
                    }
                }
                finally
                {
                    if(service != IntPtr.Zero)
                    {
                        NativeMethods.CloseServiceHandle(service);
                    }
                }
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        public static void StartService(string serviceName)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.Connect);

            try
            {
                var service = NativeMethods.OpenServiceW
                    (scm, serviceName, NativeMethods.ServiceAccessRights.QueryStatus | NativeMethods.ServiceAccessRights.Start);
                if(service == IntPtr.Zero)
                {
                    throw new ServiceException("Could not open service.");
                }

                try
                {
                    StartService(service);
                }
                finally
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        public static void StopService(string serviceName)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.Connect);

            try
            {
                var service = NativeMethods.OpenServiceW
                    (scm, serviceName, NativeMethods.ServiceAccessRights.QueryStatus | NativeMethods.ServiceAccessRights.Stop);
                if(service == IntPtr.Zero)
                {
                    throw new ServiceException("Could not open service.");
                }

                try
                {
                    StopService(service);
                }
                finally
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        private static void StartService(IntPtr service)
        {
            NativeMethods.StartService(service, 0, IntPtr.Zero);
            var changedStatus = WaitForServiceStatus
                (service, NativeMethods.ServiceState.StartPending, NativeMethods.ServiceState.Running);
            if(!changedStatus)
            {
                throw new ServiceException("Unable to start service");
            }
        }

        private static void StopService(IntPtr service)
        {
            var status = new NativeMethods.ServiceStatus();
            NativeMethods.ControlService(service, NativeMethods.ServiceControl.Stop, status);
            var changedStatus = WaitForServiceStatus
                (service, NativeMethods.ServiceState.StopPending, NativeMethods.ServiceState.Stopped);
            if(!changedStatus)
            {
                throw new ServiceException("Unable to stop service");
            }
        }

        public static NativeMethods.ServiceState GetServiceStatus(string serviceName)
        {
            var scm = OpenScManager(NativeMethods.ScmAccessRights.Connect);

            try
            {
                var service = NativeMethods.OpenServiceW(scm, serviceName, NativeMethods.ServiceAccessRights.QueryStatus);
                if(service == IntPtr.Zero)
                {
                    return NativeMethods.ServiceState.NotFound;
                }

                try
                {
                    return GetServiceStatus(service);
                }
                finally
                {
                    NativeMethods.CloseServiceHandle(service);
                }
            }
            finally
            {
                NativeMethods.CloseServiceHandle(scm);
            }
        }

        private static NativeMethods.ServiceState GetServiceStatus(IntPtr service)
        {
            var status = new NativeMethods.ServiceStatus();

            if(NativeMethods.QueryServiceStatus(service, status) == 0)
            {
                throw new ServiceException("Failed to query service status.");
            }

            return status.dwCurrentState;
        }

        private static bool WaitForServiceStatus
            (IntPtr service, NativeMethods.ServiceState waitStatus, NativeMethods.ServiceState desiredStatus)
        {
            var status = new NativeMethods.ServiceStatus();

            NativeMethods.QueryServiceStatus(service, status);
            if(status.dwCurrentState == desiredStatus)
            {
                return true;
            }

            var dwStartTickCount = Environment.TickCount;
            var dwOldCheckPoint = status.dwCheckPoint;

            while(status.dwCurrentState == waitStatus)
            {
                var dwWaitTime = status.dwWaitHint / 10;

                if(dwWaitTime < 1000)
                {
                    dwWaitTime = 1000;
                }
                else if(dwWaitTime > 10000)
                {
                    dwWaitTime = 10000;
                }

                Thread.Sleep(dwWaitTime);

                if(NativeMethods.QueryServiceStatus(service, status) == 0)
                {
                    break;
                }

                if(status.dwCheckPoint > dwOldCheckPoint)
                {
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = status.dwCheckPoint;
                }
                else
                {
                    if((Environment.TickCount - dwStartTickCount) > status.dwWaitHint)
                    {
                        break;
                    }
                }
            }

            return status.dwCurrentState == desiredStatus;
        }

        private static IntPtr OpenScManager(NativeMethods.ScmAccessRights rights)
        {
            if(rights == NativeMethods.ScmAccessRights.AllAccess && !WindowsUtils.IsAdmin())
            {
                throw new SecurityException("Administrator rights required.");
            }

            var scm = NativeMethods.OpenSCManagerW(null, null, rights);
            if(scm == IntPtr.Zero)
            {
                throw new ServiceException("Could not connect to service control manager.");
            }

            return scm;
        }

        [Serializable]
        public class ServiceException : ApplicationException
        {
            public ServiceException(string message) : base(message) {}
        }
    }
}
