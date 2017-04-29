using System;
using System.Runtime.InteropServices;

namespace Gurimu_Note_Auto_Get_Gold.Data
{
    class GetWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        /// <summary>
        /// returns handle of specified window name
        /// </summary>
        public IntPtr FindWindow(string wndName)
        {
            return FindWindow(null, wndName);
        }
        
    }
}
