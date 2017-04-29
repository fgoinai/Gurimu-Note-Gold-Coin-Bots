using System.Diagnostics;

namespace Gurimu_Note_Auto_Get_Gold.Logic
{
    static class ProcessCheck
    {
        /// <summary>
        /// To check target process exists or not
        /// </summary>
        /// <param name="name">Target Process</param>
        /// <returns>True if process exists, else false</returns>
        public static bool programExistChecking(string name)
        {
            Process[] process = Process.GetProcesses();
            foreach(Process p in process)
            {
                if(p.ProcessName == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// To get PID of process
        /// </summary>
        /// <param name="name">Target Process</param>
        /// <returns>PID</returns>
        public static int getPID(string name)
        {
            Process[] process = Process.GetProcesses();
            foreach(Process p in process)
            {
                if(p.ProcessName == name)
                {
                    return p.Id;
                }
            }
            return 0;
        }
    }
}
