using System;
using System.Diagnostics;
using System.Reflection;
using Suigetsu.Core.Common;

namespace Suigetsu.Core.Util
{
    /// <summary>
    ///     Utility methods related to Assemblies in general.
    /// </summary>
    public static class AssemblyUtils
    {
        /// <summary>
        ///     When inside a test execution, returns <see cref="M:Suigetsu.Core.Common.Testing.GetTestAssembly()" />. Otherwise,
        ///     returns the original <see cref="M:System.Reflection.Assembly.GetEntryAssembly()" />.
        /// </summary>
        public static Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly() ?? Testing.GetTestAssembly();
        }

        /// <summary>
        ///     Gets the calling type based on the current call stack.
        /// </summary>
        public static Type GetCallingType(int framesToSkip = 1)
        {
            Type declaringType;
            do
            {
                declaringType = new StackFrame(framesToSkip, false).GetMethod().DeclaringType;
                framesToSkip++;
            } while(declaringType != null && declaringType.Module.Name.ToLower() == "mscorlib.dll");

            return declaringType;
        }

        /// <summary>
        ///     Gets the calling type name based on the current call stack.
        /// </summary>
        public static string GetCallingTypeName(int framesToSkip = 1)
        {
            return GetCallingType(framesToSkip + 1)?.FullName ?? string.Empty;
        }
    }
}
