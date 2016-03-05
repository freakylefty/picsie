using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Picsie.Managers
{
    class AppManager
    {
        public static bool IsSoleInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            string applicationTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            var runningProcess = (from process in Process.GetProcesses() 
                                  where process.Id != currentProcess.Id && process.ProcessName.Equals(currentProcess.ProcessName, StringComparison.Ordinal)
                                  select process).FirstOrDefault();
            return runningProcess == null;
        }
    }
}
