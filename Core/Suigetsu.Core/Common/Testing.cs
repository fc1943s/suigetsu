using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Common
{
    /// <summary>
    ///     Methods related to unit testing. Currently supports only NUnit.
    /// </summary>
    public static class Testing
    {
        /// <summary>
        ///     Tests if the current execution was started by any supported unit test library.
        /// </summary>
        public static bool IsTestRunning()
            =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        /// <summary>
        ///     Tries to find the assembly of the test method being currently executed.
        /// </summary>
        public static Assembly GetTestAssembly()
        {
            var framesToSkip = 1;
            Type declaringType = null;

            while(true)
            {
                var frame = new StackFrame(framesToSkip, false);

                framesToSkip++;

                var currentDeclaringType = frame.GetMethod().DeclaringType;

                if(currentDeclaringType != null)
                {
                    var moduleName = currentDeclaringType.Module.Name.ToLower();

                    if(moduleName.In("nunit.core.dll", "nunit.framework.dll", "system.web.dll", "system.web.mvc.dll"))
                    {
                        break;
                    }

                    // Web Applications are tricky
                    if(moduleName != "mscorlib.dll" && currentDeclaringType.FullName != "ASP.global_asax")
                    {
                        declaringType = currentDeclaringType;
                    }
                }
            }

            return declaringType?.Assembly;
        }
    }
}
