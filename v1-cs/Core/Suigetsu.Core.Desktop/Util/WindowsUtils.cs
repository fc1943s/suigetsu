using System;
using System.Security.Principal;

namespace Suigetsu.Core.Desktop.Util
{
    public static class WindowsUtils
    {
        public static bool IsAdmin()
        {
            var currentUser = WindowsIdentity.GetCurrent();
            if(currentUser == null)
            {
                throw new ArgumentException("Current Windows user not found.");
            }

            return new WindowsPrincipal(currentUser).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
