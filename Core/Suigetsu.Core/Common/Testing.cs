using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Common
{
    public static class Testing
    {
        public static bool IsTestRunning()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("nunit.framework"));
        }

        public static Assembly GetTestAssembly()
        {
            var framesToSkip = 1;
            Type declaringType = null;

            while(true)
            {
                var frame = new StackFrame(framesToSkip, false);

                framesToSkip++;

                var method = frame.GetMethod();

                if(method.DeclaringType == null)
                {
                    continue;
                }

                var moduleName = method.DeclaringType.Module.Name.ToLower();

                if(moduleName.In("nunit.core.dll", "system.web.dll", "system.web.mvc.dll"))
                {
                    break;
                }

                if(moduleName != "mscorlib.dll" && method.DeclaringType.FullName != "ASP.global_asax")
                {
                    declaringType = method.DeclaringType;
                }
            }

            return declaringType?.Assembly;
        }
    }
}
